using CatalogDemo.API.Infrastructure.EntityConfigurations;
using CatalogDemo.API.Model;
using Microsoft.EntityFrameworkCore;

namespace CatalogDemo.API.Infrastructure
{
    public class CatalogContext : DbContext
    {        
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProductEntityTypeConfiguration());
        }


        public DbSet<Product> Products { get; set; }
    }
}
