using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions opts) : base(opts) { } 

        public DbSet<Stock> InventoryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Stock>(p => 
            { 
                p.HasKey(p => p.Id);

                p.HasIndex(p => p.ProductId)
                 .IsUnique();
            });
        }
    }
}
