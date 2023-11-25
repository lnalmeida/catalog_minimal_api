using CatalogA_PI_Minimal.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogA_PI_Minimal.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            var category = mb.Entity<Category>();
            category.HasKey(c => c.CategoryId);
            category.Property(c => c.CategoryId).HasColumnType("char(36)");
            category.Property(c => c.Name).HasMaxLength(100).IsRequired();
            category.Property(c => c.Description).HasMaxLength(350);

            var product = mb.Entity<Product>();
            product.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(c => c.CategoryId);
            product.HasKey(p => p.Id);
            product.Property(p => p.Id).HasColumnType("char(36)");
            product.Property(p => p.Name).HasMaxLength(100).IsRequired();
            product.Property(p => p.Description).HasMaxLength(350);
            product.Property(p => p.Price).HasPrecision(10, 2);
            product.Property(p => p.ImageUrl).HasMaxLength(300);


        }
    }
}
