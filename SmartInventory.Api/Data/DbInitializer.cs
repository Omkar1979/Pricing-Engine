using SmartInventory.Api.Models;

namespace SmartInventory.Api.Data;

public static class DbInitializer
{
    public static void SeedData(ApplicationDbContext context)
    {
        if (context.Products.Any())
        {
            return; // Database has been seeded
        }

        var products = new List<Product>
        {
            new Product
            {
                Name = "Laptop Computer",
                CostPrice = 800.00m,
                SellingPrice = 1200.00m,
                StockQuantity = 15,
                ReorderLevel = 20,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Wireless Mouse",
                CostPrice = 15.00m,
                SellingPrice = 25.00m,
                StockQuantity = 150,
                ReorderLevel = 30,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Mechanical Keyboard",
                CostPrice = 60.00m,
                SellingPrice = 95.00m,
                StockQuantity = 8,
                ReorderLevel = 15,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "USB-C Cable",
                CostPrice = 5.00m,
                SellingPrice = 12.00m,
                StockQuantity = 200,
                ReorderLevel = 50,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Monitor 27 inch",
                CostPrice = 300.00m,
                SellingPrice = 450.00m,
                StockQuantity = 12,
                ReorderLevel = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Webcam HD",
                CostPrice = 40.00m,
                SellingPrice = 65.00m,
                StockQuantity = 5,
                ReorderLevel = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}

