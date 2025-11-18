using Microsoft.AspNetCore.Mvc;
using SmartInventory.Api.DTOs;
using SmartInventory.Api.Services;

namespace SmartInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricingController : ControllerBase
{
    private readonly IPricingEngineService _pricingEngineService;
    private readonly ILogger<PricingController> _logger;

    public PricingController(
        IPricingEngineService pricingEngineService,
        ILogger<PricingController> logger)
    {
        _pricingEngineService = pricingEngineService;
        _logger = logger;
    }

    [HttpGet("recommend/{productId}")]
    public async Task<ActionResult<PriceRecommendationDto>> GetRecommendation(int productId)
    {
        try
        {
            var recommendation = await _pricingEngineService.GetPriceRecommendationAsync(productId);
            return Ok(recommendation);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product {ProductId} not found for price recommendation.", productId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting price recommendation for product {ProductId}.", productId);
            return StatusCode(500, "An error occurred while getting the price recommendation.");
        }
    }
}

