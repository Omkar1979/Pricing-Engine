using Microsoft.EntityFrameworkCore;
using SmartInventory.Api.Models;

namespace SmartInventory.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<PriceHistory> PriceHistories { get; set; }
    public DbSet<InventoryLog> InventoryLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            // SQLite handles decimal precision automatically
            entity.HasIndex(e => e.Name);
        });

        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.HasKey(e => e.PriceHistoryId);
            // SQLite handles decimal precision automatically
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.PriceHistories)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InventoryLog>(entity =>
        {
            entity.HasKey(e => e.InventoryLogId);
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.HasIndex(e => e.LoggedAt);
        });
    }
}

