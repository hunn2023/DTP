using DTP.Modules.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DTP.Modules.Catalog.Infrastructure.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.Sku)
                .HasMaxLength(100);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.ShortName)
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.Variants)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ProductId);

            builder.HasIndex(x => new { x.ProductId, x.Sku })
                .IsUnique()
                .HasFilter("[Sku] IS NOT NULL");

            builder.HasIndex(x => new { x.ProductId, x.SortOrder });
        }
    }
}
