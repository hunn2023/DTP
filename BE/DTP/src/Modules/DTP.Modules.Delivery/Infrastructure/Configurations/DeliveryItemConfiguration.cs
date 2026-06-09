using DTP.Modules.Delivery.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DTP.Modules.Delivery.Infrastructure.Configurations
{
    public class DeliveryItemConfiguration : IEntityTypeConfiguration<DeliveryItem>
    {
        public void Configure(EntityTypeBuilder<DeliveryItem> builder)
        {
            builder.ToTable("DeliveryItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductName)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.Sku)
                .HasMaxLength(100);

            builder.Property(x => x.QrCodeUrl)
                .HasMaxLength(2000);

            builder.Property(x => x.ActivationCode)
                .HasMaxLength(500);

            builder.Property(x => x.SerialNumber)
                .HasMaxLength(500);

            builder.Property(x => x.ProviderReference)
                .HasMaxLength(500);

            builder.Property(x => x.RawData)
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(x => x.DeliveryId);

            builder.HasIndex(x => x.OrderItemId);
        }
    }
}
