using Microsoft.EntityFrameworkCore;
using MiniERP.PurchaseOrderService.Models;

namespace MiniERP.PurchaseOrderService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions opts) : base (opts) { }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PODetail> PODetails { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PurchaseOrder>(po =>
            {
                po.HasKey(p => p.Id);

                po.Property(x => x.OrderDate)
                .HasConversion
                (
                    src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                    dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
                );

                po.Property(x => x.DeliveryDate)
                .HasConversion
                (
                    src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                    dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
                );
            });

            builder.Entity<PODetail>(pod =>
            {
                pod.HasKey(p => p.Id);

                pod.HasIndex(p => new {p.OrderNo, p.LineNo})
                   .IsUnique();

                pod.Property(x => x.LineNo)
                   .IsRequired();

                pod.HasOne<PurchaseOrder>()
                   .WithMany(po => po.Details)
                   .HasForeignKey(p => p.OrderNo);
            });
        }
    }
}
