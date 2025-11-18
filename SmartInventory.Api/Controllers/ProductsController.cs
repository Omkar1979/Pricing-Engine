using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Api.DTOs;
using SmartInventory.Api.Models;
using SmartInventory.Api.Repositories;

namespace SmartInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products.");
            return StatusCode(500, "An error occurred while retrieving products.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}.", id);
            return StatusCode(500, "An error occurred while retrieving the product.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto createProductDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(createProductDto);
            var createdProduct = await _productRepository.CreateAsync(product);
            var productDto = _mapper.Map<ProductDto>(createdProduct);

            _logger.LogInformation("Product created: {ProductName} (ID: {ProductId})", product.Name, product.ProductId);
            return CreatedAtAction(nameof(GetById), new { id = productDto.ProductId }, productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product.");
            return StatusCode(500, "An error occurred while creating the product.");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto updateProductDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            // Track old price for history
            var oldPrice = existingProduct.SellingPrice;

            _mapper.Map(updateProductDto, existingProduct);
            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);

            // Log price change if price was updated
            if (oldPrice != updatedProduct.SellingPrice)
            {
                // Price history will be handled by a service if needed
                _logger.LogInformation("Product price updated: {ProductName} (ID: {ProductId}) from ${OldPrice} to ${NewPrice}",
                    updatedProduct.Name, updatedProduct.ProductId, oldPrice, updatedProduct.SellingPrice);
            }

            var productDto = _mapper.Map<ProductDto>(updatedProduct);
            return Ok(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}.", id);
            return StatusCode(500, "An error occurred while updating the product.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _productRepository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            _logger.LogInformation("Product deleted: ID {ProductId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}.", id);
            return StatusCode(500, "An error occurred while deleting the product.");
        }
    }
}

