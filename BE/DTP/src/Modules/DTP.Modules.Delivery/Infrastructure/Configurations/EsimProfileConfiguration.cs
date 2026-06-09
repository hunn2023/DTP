using DTP.Modules.Delivery.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DTP.Modules.Delivery.Infrastructure.Configurations
{
    public class EsimProfileConfiguration : IEntityTypeConfiguration<EsimProfile>
    {
        public void Configure(EntityTypeBuilder<EsimProfile> builder)
        {
            builder.ToTable("EsimProfiles", "delivery");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Iccid)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(x => x.Iccid)
                .IsUnique();

            builder.Property(x => x.Imsi)
                .HasMaxLength(100);

            builder.Property(x => x.Msisdn)
                .HasMaxLength(100);

            builder.Property(x => x.ActivationCode)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.QrCodeUrl)
                .HasMaxLength(2000);

            builder.Property(x => x.QrContent)
                .HasMaxLength(4000);

            builder.Property(x => x.SmdpAddress)
                .HasMaxLength(500);

            builder.Property(x => x.MatchingId)
                .HasMaxLength(500);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => x.ProductVariantId);
            builder.HasIndex(x => x.EsimPackageId);
            builder.HasIndex(x => x.ProviderId);
            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.OrderItemId);
            builder.HasIndex(x => x.Status);
        }
    }
}
