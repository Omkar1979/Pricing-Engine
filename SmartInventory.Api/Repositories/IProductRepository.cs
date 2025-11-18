using SmartInventory.Api.Models;

namespace SmartInventory.Api.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Product>> GetProductsBelowReorderLevelAsync();
    Task<bool> ExistsAsync(int id);
}

