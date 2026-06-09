using DTP.Modules.Ordering.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DTP.Modules.Ordering.Infrastructure.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems", "ordering");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ItemType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.ProductName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.VariantName)
                .HasMaxLength(255);

            builder.Property(x => x.Sku)
                .HasMaxLength(100);

            builder.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Currency)
                .HasMaxLength(10)
                .IsRequired();

            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => x.ProductVariantId);
            builder.HasIndex(x => x.EsimPackageId);
            builder.HasIndex(x => x.PhoneCardId);
        }
    }
}
