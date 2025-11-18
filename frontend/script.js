// Update this URL to match your API endpoint
// Default: HTTP on port 5000, or change to 'https://localhost:7000/api' for HTTPS
const API_BASE_URL = 'http://localhost:5000/api';

let currentEditingProductId = null;

// Load products on page load
document.addEventListener('DOMContentLoaded', () => {
    loadProducts();
});

// Load all products
async function loadProducts() {
    try {
        const response = await fetch(`${API_BASE_URL}/products`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const products = await response.json();
        displayProducts(products);
    } catch (error) {
        console.error('Error loading products:', error);
        document.getElementById('productsTableBody').innerHTML = 
            '<tr><td colspan="7" class="loading" style="color: red;">Error loading products. Please check if the API is running.</td></tr>';
    }
}

// Display products in table
function displayProducts(products) {
    const tbody = document.getElementById('productsTableBody');
    
    if (products.length === 0) {
        tbody.innerHTML = '<tr><td colspan="7" class="loading">No products found. Add your first product!</td></tr>';
        return;
    }

    tbody.innerHTML = products.map(product => `
        <tr>
            <td>${product.productId}</td>
            <td>${escapeHtml(product.name)}</td>
            <td>$${product.costPrice.toFixed(2)}</td>
            <td>$${product.sellingPrice.toFixed(2)}</td>
            <td>${product.stockQuantity}</td>
            <td>${product.reorderLevel}</td>
            <td>
                <button class="btn btn-info" onclick="getPriceRecommendation(${product.productId})">Get Recommended Price</button>
                <button class="btn btn-edit" onclick="editProduct(${product.productId})">Edit</button>
                <button class="btn btn-danger" onclick="deleteProduct(${product.productId}, '${escapeHtml(product.name)}')">Delete</button>
            </td>
        </tr>
    `).join('');
}

// Get price recommendation
async function getPriceRecommendation(productId) {
    try {
        const response = await fetch(`${API_BASE_URL}/pricing/recommend/${productId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || `HTTP error! status: ${response.status}`);
        }

        const recommendation = await response.json();
        showRecommendationModal(recommendation);
    } catch (error) {
        console.error('Error getting price recommendation:', error);
        alert('Error getting price recommendation: ' + error.message);
    }
}

// Show recommendation modal
function showRecommendationModal(recommendation) {
    document.getElementById('currentPrice').textContent = `$${recommendation.currentPrice.toFixed(2)}`;
    document.getElementById('recommendedPrice').textContent = `$${recommendation.recommendedPrice.toFixed(2)}`;
    
    const reasonsList = document.getElementById('reasonsList');
    if (recommendation.reasons && recommendation.reasons.length > 0) {
        reasonsList.innerHTML = recommendation.reasons.map(reason => 
            `<li>${escapeHtml(reason)}</li>`
        ).join('');
    } else {
        reasonsList.innerHTML = '<li>No specific recommendations. Current price is optimal.</li>';
    }
    
    document.getElementById('recommendationModal').style.display = 'block';
}

// Close recommendation modal
function closeRecommendationModal() {
    document.getElementById('recommendationModal').style.display = 'none';
}

// Open add product modal
function openAddProductModal() {
    currentEditingProductId = null;
    document.getElementById('modalTitle').textContent = 'Add New Product';
    document.getElementById('productForm').reset();
    document.getElementById('productModal').style.display = 'block';
}

// Edit product
async function editProduct(productId) {
    try {
        const response = await fetch(`${API_BASE_URL}/products/${productId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const product = await response.json();
        currentEditingProductId = productId;
        
        document.getElementById('modalTitle').textContent = 'Edit Product';
        document.getElementById('productName').value = product.name;
        document.getElementById('costPrice').value = product.costPrice;
        document.getElementById('sellingPrice').value = product.sellingPrice;
        document.getElementById('stockQuantity').value = product.stockQuantity;
        document.getElementById('reorderLevel').value = product.reorderLevel;
        
        document.getElementById('productModal').style.display = 'block';
    } catch (error) {
        console.error('Error loading product for edit:', error);
        alert('Error loading product: ' + error.message);
    }
}

// Close product modal
function closeProductModal() {
    document.getElementById('productModal').style.display = 'none';
    currentEditingProductId = null;
    document.getElementById('productForm').reset();
}

// Handle product form submit
async function handleProductSubmit(event) {
    event.preventDefault();
    
    const formData = {
        name: document.getElementById('productName').value,
        costPrice: parseFloat(document.getElementById('costPrice').value),
        sellingPrice: parseFloat(document.getElementById('sellingPrice').value),
        stockQuantity: parseInt(document.getElementById('stockQuantity').value),
        reorderLevel: parseInt(document.getElementById('reorderLevel').value)
    };

    try {
        const url = currentEditingProductId 
            ? `${API_BASE_URL}/products/${currentEditingProductId}`
            : `${API_BASE_URL}/products`;
        
        const method = currentEditingProductId ? 'PUT' : 'POST';

        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || `HTTP error! status: ${response.status}`);
        }

        closeProductModal();
        loadProducts();
        
        const action = currentEditingProductId ? 'updated' : 'created';
        alert(`Product ${action} successfully!`);
    } catch (error) {
        console.error('Error saving product:', error);
        alert('Error saving product: ' + error.message);
    }
}

// Delete product
async function deleteProduct(productId, productName) {
    if (!confirm(`Are you sure you want to delete "${productName}"?`)) {
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/products/${productId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || `HTTP error! status: ${response.status}`);
        }

        loadProducts();
        alert('Product deleted successfully!');
    } catch (error) {
        console.error('Error deleting product:', error);
        alert('Error deleting product: ' + error.message);
    }
}

// Close modals when clicking outside
window.onclick = function(event) {
    const productModal = document.getElementById('productModal');
    const recommendationModal = document.getElementById('recommendationModal');
    
    if (event.target === productModal) {
        closeProductModal();
    }
    if (event.target === recommendationModal) {
        closeRecommendationModal();
    }
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

