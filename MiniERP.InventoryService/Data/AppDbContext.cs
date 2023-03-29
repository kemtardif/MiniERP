using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions opts) : base(opts) { } 

        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Stock> Stocks { get; set; }

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
        }
    }
}
