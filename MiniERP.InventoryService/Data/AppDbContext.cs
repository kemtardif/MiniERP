using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Models.Views;

namespace MiniERP.InventoryService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions opts) : base(opts) { } 

        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<InventoryMovement> StockMovements { get; set; }
        public DbSet<AvailableInventoryView> AvailableInventoryView { get; set; }
        public DbSet<PendingStockView> PendingStockView { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<InventoryItem>(p => 
            { 
                p.HasKey(p => p.Id);

                p.HasIndex(p => p.ArticleId)
                 .IsUnique();
                    
            });


            builder.Entity<InventoryMovement>(sm => 
            {
                sm.HasKey(p => p.Id);

                sm.HasOne(x => x.InventoryItem)
                .WithMany()
                .IsRequired();
             
            });

            builder.Entity<AvailableInventoryView>(ism =>
            {
                ism.ToView(nameof(AvailableInventoryView))
                .HasKey(x => x.ArticleId);
            });

            builder.Entity<PendingStockView>(ism =>
            {
                ism.ToView(nameof(PendingStockView))
                .HasKey(x => x.ArticleId);
            });

        }
    }
}
