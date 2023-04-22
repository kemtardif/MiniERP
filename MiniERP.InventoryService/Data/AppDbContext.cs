using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Models;

namespace MiniERP.InventoryService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions opts) : base(opts) { } 

        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<InventoryMovement> StockMovements { get; set; }

        public DbSet<AvailableInventoryView> AvailableInventoryView { get; set; }
        public DbSet<PendingInventoryView> PendingInventoryView { get; set; }

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
                .HasForeignKey(x => x.InventoryId)
                .HasPrincipalKey(x => x.Id)
                .IsRequired();
             
            });

            builder.Entity<AvailableInventoryView>(ism =>
            {
                ism.ToView(nameof(AvailableInventoryView))
                .HasKey(x => x.ArticleId);
            });

            builder.Entity<PendingInventoryView>(piv =>
            {
                piv.ToView(nameof(PendingInventoryView))
                .HasKey(x => x.ArticleId);
            });
        }
    }
}
