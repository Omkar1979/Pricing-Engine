using Microsoft.Extensions.Caching.Memory;
using SmartInventory.Api.DTOs;
using SmartInventory.Api.Models;
using SmartInventory.Api.Repositories;

namespace SmartInventory.Api.Services;

public class PricingEngineService : IPricingEngineService
{
    private readonly IProductRepository _productRepository;
    private readonly IMemoryCache _cache;
    private const decimal MinimumProfitMargin = 0.10m; // 10%
    private const decimal LowStockPriceIncrease = 0.10m; // 10%
    private const decimal HighStockPriceDecrease = 0.05m; // 5%
    private const decimal CompetitorPriceAdjustment = 0.05m; // 5%

    public PricingEngineService(IProductRepository productRepository, IMemoryCache cache)
    {
        _productRepository = productRepository;
        _cache = cache;
    }

    public async Task<PriceRecommendationDto> GetPriceRecommendationAsync(int productId)
    {
        var cacheKey = $"price_recommendation_{productId}";
        
        if (_cache.TryGetValue(cacheKey, out PriceRecommendationDto? cachedRecommendation))
        {
            return cachedRecommendation!;
        }

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {productId} not found.");
        }

        var currentPrice = product.SellingPrice;
        var recommendedPrice = currentPrice;
        var reasons = new List<string>();

        // Rule 1: If StockQuantity < ReorderLevel → increase price by 10%
        if (product.StockQuantity < product.ReorderLevel)
        {
            var increase = currentPrice * LowStockPriceIncrease;
            recommendedPrice += increase;
            reasons.Add($"Stock quantity ({product.StockQuantity}) is below reorder level ({product.ReorderLevel}). Increasing price by 10%.");
        }

        // Rule 2: If StockQuantity > 100 → decrease price by 5%
        if (product.StockQuantity > 100)
        {
            var decrease = currentPrice * HighStockPriceDecrease;
            recommendedPrice -= decrease;
            reasons.Add($"Stock quantity ({product.StockQuantity}) is above 100. Decreasing price by 5%.");
        }

        // Rule 3: Ensure selling price maintains minimum 10% profit margin over cost price
        var minimumPrice = product.CostPrice * (1 + MinimumProfitMargin);
        if (recommendedPrice < minimumPrice)
        {
            recommendedPrice = minimumPrice;
            reasons.Add($"Adjusted to maintain minimum 10% profit margin over cost price.");
        }

        // Rule 4: Compare with dummy competitor prices
        var competitorPrices = new[]
        {
            currentPrice - 20,
            currentPrice + 20,
            currentPrice - 10
        };

        var averageCompetitorPrice = competitorPrices.Average();
        var priceDifference = (recommendedPrice - averageCompetitorPrice) / averageCompetitorPrice;

        // If product price is significantly higher (more than 5% above average) → reduce by 5%
        if (priceDifference > 0.05m)
        {
            var adjustment = recommendedPrice * CompetitorPriceAdjustment;
            recommendedPrice -= adjustment;
            reasons.Add($"Price is significantly higher than competitor average (${averageCompetitorPrice:F2}). Reducing by 5%.");
        }

        // Final check: Ensure minimum profit margin is maintained
        if (recommendedPrice < minimumPrice)
        {
            recommendedPrice = minimumPrice;
            if (!reasons.Contains("Adjusted to maintain minimum 10% profit margin over cost price."))
            {
                reasons.Add($"Final adjustment to maintain minimum 10% profit margin over cost price.");
            }
        }

        // Round to 2 decimal places
        recommendedPrice = Math.Round(recommendedPrice, 2);

        var recommendation = new PriceRecommendationDto
        {
            CurrentPrice = currentPrice,
            RecommendedPrice = recommendedPrice,
            Reasons = reasons
        };

        // Cache for 30 minutes
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };
        _cache.Set(cacheKey, recommendation, cacheOptions);

        return recommendation;
    }
}

