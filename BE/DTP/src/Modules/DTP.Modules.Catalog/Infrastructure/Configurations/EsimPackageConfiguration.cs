using DTP.Modules.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DTP.Modules.Catalog.Infrastructure.Persistence.Configurations
{
    public class EsimPackageConfiguration : IEntityTypeConfiguration<EsimPackage>
    {
        public void Configure(EntityTypeBuilder<EsimPackage> builder)
        {
            builder.ToTable("EsimPackages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.ProductVariantId)
                .IsRequired();

            builder.Property(x => x.ProviderId)
                .IsRequired();

            builder.Property(x => x.CountryId)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.Slug)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.ProviderPackageCode)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.DataAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DataUnit)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.ValidityDays)
                .IsRequired();

            builder.Property(x => x.IsUnlimited)
                .IsRequired();

            builder.Property(x => x.CoverageType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.CoverageDescription)
                .HasMaxLength(1000);

            builder.Property(x => x.ActivationPolicy)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.SpeedPolicy)
                .HasMaxLength(500);

            builder.Property(x => x.HotspotSupported)
                .IsRequired();

            builder.Property(x => x.PhoneNumberSupported)
                .IsRequired();

            builder.Property(x => x.SmsSupported)
                .IsRequired();

            builder.Property(x => x.KycRequired)
                .IsRequired();

            builder.Property(x => x.QrDeliveryType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.ProductVariant)
                .WithMany()
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Carriers)
                .WithOne(x => x.EsimPackage)
                .HasForeignKey(x => x.EsimPackageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.Carriers)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(x => x.Slug)
                .IsUnique();

            builder.HasIndex(x => x.ProviderPackageCode);

            builder.HasIndex(x => new
            {
                x.ProductId,
                x.ProductVariantId,
                x.ProviderId,
                x.CountryId
            });

            builder.HasIndex(x => new
            {
                x.CountryId,
                x.IsActive,
                x.SortOrder
            });
        }
    }
}