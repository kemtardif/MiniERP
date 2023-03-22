using Microsoft.EntityFrameworkCore;
using MiniERP.SalesOrderService.Models;

namespace MiniERP.SalesOrderService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions opts) : base(opts){ }

        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SalesOrder>(so =>
            {
                so.HasKey(x => x.Id);

                so.HasIndex(x => x.Id)
                .IsUnique();
              
            });

            builder.Entity<SalesOrderDetail>(so =>
            {
                so.HasKey(x => x.Id);

                so.HasIndex(x => x.Id)
                .IsUnique();

                so.HasOne<SalesOrder>()
                .WithMany(x => x.Details)
                .HasForeignKey(y => y.SalesOrderId);

            });
                   
        }
    }
}
