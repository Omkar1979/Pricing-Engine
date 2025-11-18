namespace SmartInventory.Api.DTOs;

public class PriceRecommendationDto
{
    public decimal CurrentPrice { get; set; }
    public decimal RecommendedPrice { get; set; }
    public List<string> Reasons { get; set; } = new List<string>();
}

