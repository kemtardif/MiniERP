using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions opts) : base(opts) { } 

        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<InventoryItem>(p => 
            { 
                p.HasKey(p => p.Id);

                p.HasIndex(p => p.ProductId)
                 .IsUnique();

                p.HasOne(p => p.Stock)
                .WithOne()
                .HasForeignKey<Stock>(p => p.InventoryId)
                .IsRequired();
                    
            });

            builder.Entity<Stock>(p => 
            {
                p.HasKey(p => p.Id);

                p.HasIndex(p => p.InventoryId)
                 .IsUnique();
            });

            builder.Entity<StockMovement>(sm => 
            {
                sm.HasKey(p => p.Id);

                sm.HasOne<InventoryItem>()
                .WithMany(p => p.StockMovements)
                .HasForeignKey(x => x.ArticleId)
                .IsRequired();
            });
        }
    }
}
