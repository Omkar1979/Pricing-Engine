using SmartInventory.Api.DTOs;

namespace SmartInventory.Api.Services;

public interface IPricingEngineService
{
    Task<PriceRecommendationDto> GetPriceRecommendationAsync(int productId);
}

