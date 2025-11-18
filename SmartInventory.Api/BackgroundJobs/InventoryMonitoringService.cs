using Microsoft.Extensions.Caching.Memory;
using SmartInventory.Api.Data;
using SmartInventory.Api.Models;
using SmartInventory.Api.Repositories;
using SmartInventory.Api.Services;

namespace SmartInventory.Api.BackgroundJobs;

public class InventoryMonitoringService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InventoryMonitoringService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public InventoryMonitoringService(
        IServiceProvider serviceProvider,
        ILogger<InventoryMonitoringService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Inventory Monitoring Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckInventoryLevelsAsync();
                await CachePriceRecommendationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Inventory Monitoring Service.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CheckInventoryLevelsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var productsBelowReorderLevel = await productRepository.GetProductsBelowReorderLevelAsync();

        _logger.LogInformation("Checking inventory levels. Found {Count} products below reorder level.", productsBelowReorderLevel.Count());

        foreach (var product in productsBelowReorderLevel)
        {
            var log = new InventoryLog
            {
                ProductId = product.ProductId,
                ProductName = product.Name,
                StockQuantity = product.StockQuantity,
                ReorderLevel = product.ReorderLevel,
                Message = $"Product '{product.Name}' has stock quantity ({product.StockQuantity}) below reorder level ({product.ReorderLevel}).",
                LoggedAt = DateTime.UtcNow
            };

            dbContext.InventoryLogs.Add(log);
            _logger.LogWarning("Product '{ProductName}' (ID: {ProductId}) is below reorder level. Stock: {StockQuantity}, Reorder Level: {ReorderLevel}",
                product.Name, product.ProductId, product.StockQuantity, product.ReorderLevel);
        }

        await dbContext.SaveChangesAsync();
        _logger.LogInformation("Inventory check completed. Logged {Count} entries.", productsBelowReorderLevel.Count());
    }

    private async Task CachePriceRecommendationsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var pricingService = scope.ServiceProvider.GetRequiredService<IPricingEngineService>();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

        var products = await productRepository.GetAllAsync();

        _logger.LogInformation("Caching price recommendations for {Count} products.", products.Count());

        foreach (var product in products)
        {
            try
            {
                var recommendation = await pricingService.GetPriceRecommendationAsync(product.ProductId);
                var cacheKey = $"price_recommendation_{product.ProductId}";
                
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                
                memoryCache.Set(cacheKey, recommendation, cacheOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching price recommendation for product {ProductId}.", product.ProductId);
            }
        }

        _logger.LogInformation("Price recommendations cached successfully.");
    }
}

