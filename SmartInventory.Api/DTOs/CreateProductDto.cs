using System.ComponentModel.DataAnnotations;

namespace SmartInventory.Api.DTOs;

public class CreateProductDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Cost price must be greater than 0")]
    public decimal CostPrice { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Selling price must be greater than 0")]
    public decimal SellingPrice { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; }
}

