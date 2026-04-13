# Smart Inventory & Pricing Engine

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://docs.microsoft.com/dotnet/csharp/)
[![SQLite](https://img.shields.io/badge/SQLite-3.43-003B57?logo=sqlite)](https://www.sqlite.org/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A full-stack .NET 8 Web API application with clean architecture implementation for managing products and intelligent price optimization. Features a rule-based pricing engine that automatically adjusts prices based on inventory levels, profit margins, and competitor analysis.

## ✨ Features

### 🛍️ Products Module (CRUD)
- Complete CRUD operations for products
- Product fields: ProductId, Name, CostPrice, SellingPrice, StockQuantity, ReorderLevel
- RESTful API endpoints with DTOs and AutoMapper
- Input validation and error handling

### 💰 Price Optimization Engine
- **Rule-based pricing recommendations:**
  - Low stock (below reorder level) → Increase price by 10%
  - High stock (above 100) → Decrease price by 5%
  - Maintains minimum 10% profit margin over cost price
  - Competitor price comparison and adjustment
- Cached recommendations using MemoryCache (30-minute cache)
- Returns detailed reasoning for each recommendation

### 🔄 Background Service
- Runs automatically every 1 hour
- Monitors products below reorder level
- Logs inventory warnings to database
- Pre-caches price recommendations for better performance

### 📊 Logging
- Serilog integration for structured logging
- File and console logging
- Logs stored in `/Logs` directory with daily rotation

## 🏗️ Architecture

- **Backend**: .NET 8 Web API
- **Database**: SQLite (no installation required - included with .NET)
- **Frontend**: Pure HTML, CSS, and Vanilla JavaScript
- **Architecture Pattern**: Clean Architecture with Repository Pattern
- **ORM**: Entity Framework Core
- **Mapping**: AutoMapper
- **Logging**: Serilog

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (required)
- Modern web browser (Chrome, Edge, Firefox)
- Git (for cloning the repository)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/Omkar1979/smart-inventory-pricing-engine.git
   cd smart-inventory-pricing-engine
   ```

2. **Navigate to the API project**
   ```bash
   cd SmartInventory.Api
   ```

3. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

   The API will start on:
   - **HTTP**: `http://localhost:5000` ✅ (Primary)
   - **HTTPS**: `https://localhost:7000` (May require SSL certificate setup)

5. **Database is created automatically** with seed data (6 sample products)

### Frontend Setup

1. **Update API URL** (if needed)
   - Open `frontend/script.js`
   - The default URL is already set to `http://localhost:5000/api`
   - If your API runs on a different port, update the `API_BASE_URL` constant

2. **Serve the frontend**
   
   **Option 1: Using Python** (Recommended)
   ```bash
   cd frontend
   python -m http.server 8080
   ```
   
   **Option 2: Using Node.js**
   ```bash
   npx http-server frontend -p 8080
   ```
   
   **Option 3: Open directly**
   - Navigate to `frontend` folder
   - Double-click `index.html`
   - ⚠️ Note: May have CORS issues when opened directly

3. **Access the application**
   - Open your browser and navigate to `http://localhost:8080` (or your chosen port)

### Access Swagger UI

Once the API is running, you can explore and test the API using Swagger UI:

```
http://localhost:5000/swagger
```

## 📁 Project Structure

```
SmartInventory.Api/
├── Controllers/          # API Controllers
│   ├── ProductsController.cs
│   └── PricingController.cs
├── Services/             # Business Logic Services
│   ├── IPricingEngineService.cs
│   └── PricingEngineService.cs
├── Repositories/         # Data Access Layer
│   ├── IProductRepository.cs
│   └── ProductRepository.cs
├── Models/               # Entity Models
│   ├── Product.cs
│   ├── PriceHistory.cs
│   └── InventoryLog.cs
├── DTOs/                 # Data Transfer Objects
│   ├── ProductDto.cs
│   ├── CreateProductDto.cs
│   ├── UpdateProductDto.cs
│   └── PriceRecommendationDto.cs
├── Data/                 # Database Context
│   ├── ApplicationDbContext.cs
│   └── DbInitializer.cs
├── Mappers/              # AutoMapper Profiles
│   └── MappingProfile.cs
├── BackgroundJobs/       # Background Services
│   └── InventoryMonitoringService.cs
├── Logs/                 # Log files (generated at runtime)
├── Program.cs            # Application entry point
├── appsettings.json      # Configuration
└── SmartInventory.Api.csproj

frontend/
├── index.html            # Main HTML file
├── script.js             # Frontend JavaScript
└── style.css             # Styling
```

## 🔌 API Endpoints

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/products` | Get all products |
| `GET` | `/api/products/{id}` | Get product by ID |
| `POST` | `/api/products` | Create new product |
| `PUT` | `/api/products/{id}` | Update product |
| `DELETE` | `/api/products/{id}` | Delete product |

### Pricing

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/pricing/recommend/{productId}` | Get price recommendation for a product |

### Example Request/Response

**Get Price Recommendation:**
```http
GET /api/pricing/recommend/1
```

**Response:**
```json
{
  "currentPrice": 1200.00,
  "recommendedPrice": 1320.00,
  "reasons": [
    "Stock quantity (15) is below reorder level (20). Increasing price by 10%.",
    "Adjusted to maintain minimum 10% profit margin over cost price."
  ]
}
```

## 🗄️ Database Schema

### Products
- `ProductId` (int, PK) - Unique identifier
- `Name` (string, required) - Product name
- `CostPrice` (decimal) - Cost price
- `SellingPrice` (decimal) - Current selling price
- `StockQuantity` (int) - Current stock level
- `ReorderLevel` (int) - Reorder threshold
- `CreatedAt` (datetime) - Creation timestamp
- `UpdatedAt` (datetime) - Last update timestamp

### PriceHistory
- `PriceHistoryId` (int, PK) - Unique identifier
- `ProductId` (int, FK) - Reference to Product
- `OldPrice` (decimal) - Previous price
- `NewPrice` (decimal) - New price
- `ChangedAt` (datetime) - Change timestamp
- `Reason` (string) - Reason for price change

### InventoryLogs
- `InventoryLogId` (int, PK) - Unique identifier
- `ProductId` (int) - Reference to Product
- `ProductName` (string) - Product name (denormalized)
- `StockQuantity` (int) - Stock level at log time
- `ReorderLevel` (int) - Reorder threshold
- `Message` (string) - Log message
- `LoggedAt` (datetime) - Log timestamp

## 🧪 Testing the Application

1. **Start the API**
   ```bash
   cd SmartInventory.Api
   dotnet run
   ```

2. **Open the frontend** in your browser

3. **Test CRUD operations:**
   - ✅ View all products (loaded automatically)
   - ✅ Add a new product
   - ✅ Edit an existing product
   - ✅ Delete a product

4. **Test Price Recommendations:**
   - Click "Get Recommended Price" on any product
   - Review the recommendation with detailed reasons

5. **Check Background Service:**
   - Wait for the background service to run (runs every hour)
   - Check the `InventoryLogs` table for entries
   - Check the `Logs` folder for Serilog entries

## 📝 Configuration

### Connection String

The default connection string uses SQLite (no configuration needed):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SmartInventoryDb.db"
  }
}
```

The database file (`SmartInventoryDb.db`) is created automatically in the `SmartInventory.Api` folder.

### CORS Configuration

The API is configured to allow requests from common frontend ports. To add custom origins, update `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:8080", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

## 🔧 Troubleshooting

### "No .NET SDKs were found"
- **Solution**: Install [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Verify installation: `dotnet --version` should show `8.0.x` or higher

### CORS Errors in Browser
- **Solution**: Ensure the frontend URL is added to the CORS policy in `Program.cs`
- Check that the API is running and accessible
- Verify `API_BASE_URL` in `frontend/script.js` matches your API URL

### Database Connection Issues
- **Solution**: SQLite database file (`SmartInventoryDb.db`) is created automatically
- If you see database errors, delete the `.db` file and restart the application
- The database will be recreated with seed data

### API Not Responding
- **Solution**: 
  - Check if the API is running: `netstat -ano | findstr :5000`
  - Verify the API URL in `frontend/script.js` matches the running API
  - Check browser console for errors
  - Ensure you're using `http://localhost:5000` (not HTTPS)

### Port Already in Use
- **Solution**: 
  - Change ports in `Properties/launchSettings.json`
  - Or kill the process using the port

## 📦 Dependencies

### Backend Packages

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore | 8.0.0 | ORM Framework |
| Microsoft.EntityFrameworkCore.Sqlite | 8.0.0 | SQLite Provider |
| Microsoft.EntityFrameworkCore.Tools | 8.0.0 | EF Core Tools |
| AutoMapper | 12.0.1 | Object Mapping |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | AutoMapper DI |
| Serilog.AspNetCore | 8.0.0 | Logging Framework |
| Serilog.Sinks.File | 5.0.0 | File Logging |
| Serilog.Sinks.Console | 5.0.0 | Console Logging |
| Swashbuckle.AspNetCore | 6.5.0 | Swagger/OpenAPI |

## 🎯 Key Features Explained

### Price Optimization Rules

1. **Low Stock Rule**: When `StockQuantity < ReorderLevel`
   - Action: Increase price by 10%
   - Reason: Low inventory suggests high demand

2. **High Stock Rule**: When `StockQuantity > 100`
   - Action: Decrease price by 5%
   - Reason: High inventory suggests low demand

3. **Profit Margin Protection**: Always ensures `SellingPrice >= CostPrice * 1.10`
   - Maintains minimum 10% profit margin

4. **Competitor Analysis**: Compares with dummy competitor prices
   - If significantly higher than average → Reduce by 5%

### Background Service

The `InventoryMonitoringService` runs every hour and:
- Scans all products for low stock levels
- Creates inventory log entries
- Pre-caches price recommendations
- Logs all activities using Serilog

## 📄 License

This project is provided as-is for demonstration and educational purposes.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Support

For issues, questions, or suggestions, please open an issue on GitHub.

---

**Made with ❤️ using .NET 8**
