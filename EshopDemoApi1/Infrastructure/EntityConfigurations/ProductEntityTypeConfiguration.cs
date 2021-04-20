using CatalogDemo.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogDemo.API.Infrastructure.EntityConfigurations
{
    public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Catalog");

            builder.Property(ci => ci.Id)
                .UseHiLo("catalog_hilo")
                .IsRequired();

            builder.Property(ci => ci.Name)
                .IsRequired(true);

            builder.Property(ci => ci.Price)
                .HasColumnType("DECIMAL(18,2)")                
                .IsRequired(true);

            builder.Property(ci => ci.ImgUri)
                .IsRequired(true);

            builder.Property(ci => ci.Description)
                .IsRequired(false);

        }
    }
}
