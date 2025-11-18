namespace SmartInventory.Api.Models;

public class PriceHistory
{
    public int PriceHistoryId { get; set; }
    public int ProductId { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string Reason { get; set; } = string.Empty;

    // Navigation property
    public virtual Product Product { get; set; } = null!;
}

