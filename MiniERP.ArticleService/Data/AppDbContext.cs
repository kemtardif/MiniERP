using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MiniERP.ArticleService.Models;

namespace MiniERP.ArticleService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions opts) : base(opts) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Unit> Units { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Unit>(u =>
            {
                u.HasKey(x => x.Id);
            });   
            
            builder.Entity<Article>(a => 
            {
                a.HasKey(a => a.Id);

                a.HasIndex(a => a.Id)
                 .IsUnique();
            });
        }
    }
}
