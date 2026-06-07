using DTP.Modules.Ordering.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DTP.Modules.Ordering.Infrastructure.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // Add missing using for Microsoft.EntityFrameworkCore if not present.
            builder.ToTable("OrderItems", "ordering");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductCode)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ProductName)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.ProductSlug)
                .HasMaxLength(500);

            builder.Property(x => x.VariantName)
                .HasMaxLength(255);

            builder.Property(x => x.Sku)
                .HasMaxLength(100);

            builder.Property(x => x.ThumbnailUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.CurrencyCode)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => x.ProductVariantId);
            builder.HasIndex(x => x.EsimPackageId);
        }
    }
}
