# 🚀 Quick Start Guide - Smart Inventory & Pricing Engine

## Step 1: Start the API Server

Open PowerShell in the project folder and run:

```powershell
cd "D:\pricing engine\SmartInventory.Api"
dotnet run
```

**Wait for this message:**
```
Now listening on: http://localhost:5000
Now listening on: https://localhost:7000
```

✅ **Keep this terminal window open** - the API must stay running!

---

## Step 2: Open the Frontend

### Option A: Open HTML File Directly
1. Navigate to: `D:\pricing engine\frontend`
2. Double-click `index.html`
3. It will open in your default browser

### Option B: Use a Local Web Server (Recommended)
1. Open a **new** PowerShell window
2. Run:
   ```powershell
   cd "D:\pricing engine\frontend"
   python -m http.server 8080
   ```
3. Open browser to: `http://localhost:8080`

---

## Step 3: Use the Application

### View Products
- The product table will load automatically
- You'll see 6 sample products that were seeded

### Add a New Product
1. Click **"Add New Product"** button
2. Fill in the form:
   - Product Name
   - Cost Price
   - Selling Price
   - Stock Quantity
   - Reorder Level
3. Click **"Save Product"**

### Edit a Product
1. Click **"Edit"** button next to any product
2. Modify the fields
3. Click **"Save Product"**

### Delete a Product
1. Click **"Delete"** button next to any product
2. Confirm deletion

### Get Price Recommendation
1. Click **"Get Recommended Price"** button next to any product
2. A modal will show:
   - Current Price
   - Recommended Price
   - Reasons for the recommendation

---

## Step 4: Test the API Directly (Optional)

### Using Swagger UI
1. Open browser to: `http://localhost:5000/swagger` ⚠️ Use **HTTP** not HTTPS
2. Explore all available endpoints
3. Test API calls directly from the browser

### Available Endpoints:

**Products:**
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

**Pricing:**
- `GET /api/pricing/recommend/{productId}` - Get price recommendation

---

## Understanding Price Recommendations

The pricing engine uses these rules:

1. **Low Stock Rule**: If stock is below reorder level → Price increases by 10%
2. **High Stock Rule**: If stock is above 100 → Price decreases by 5%
3. **Profit Margin**: Always maintains minimum 10% profit over cost price
4. **Competitor Analysis**: Compares with dummy competitor prices and adjusts if needed

---

## Background Service

The application includes a background service that:
- Runs every 1 hour automatically
- Checks products below reorder level
- Logs inventory warnings to the database
- Caches price recommendations

You can see these logs in:
- Console output (where you ran `dotnet run`)
- Log files in: `SmartInventory.Api/Logs/`

---

## Troubleshooting

### "Cannot connect to API"
- Make sure the API is running (Step 1)
- Check the API URL in `frontend/script.js` matches your running API
- Default: `http://localhost:5000/api`

### "CORS Error" in Browser
- The API is configured for common ports
- If using a different port, update CORS in `Program.cs`

### Database File
- The database file `SmartInventoryDb.db` is created automatically
- Location: `D:\pricing engine\SmartInventory.Api\SmartInventoryDb.db`
- To reset: Delete this file and restart the application

---

## Stopping the Application

1. Go to the terminal where `dotnet run` is executing
2. Press `Ctrl + C` to stop the API server
3. If using a web server for frontend, press `Ctrl + C` there too

---

## Next Steps

- ✅ Add more products
- ✅ Test price recommendations with different stock levels
- ✅ Check inventory logs after the background service runs (every hour)
- ✅ Explore the Swagger UI to understand the API structure

Enjoy using the Smart Inventory & Pricing Engine! 🎉

