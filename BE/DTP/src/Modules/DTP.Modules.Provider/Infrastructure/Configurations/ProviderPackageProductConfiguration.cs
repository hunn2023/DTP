using DTP.Modules.Provider.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Configurations
{
    public class ProviderPackageProductConfiguration
    : IEntityTypeConfiguration<ProviderPackageProduct>
    {
        public void Configure(EntityTypeBuilder<ProviderPackageProduct> builder)
        {
            builder.ToTable("ProviderPackageProducts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProviderSku)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ProviderProductId)
                .HasMaxLength(100);

            builder.Property(x => x.Name)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.Model)
                .HasMaxLength(255);

            builder.Property(x => x.Regional)
                .HasMaxLength(255);

            builder.Property(x => x.CurrencyCode)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Price)
                .HasPrecision(18, 2);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(2000);

            builder.Property(x => x.SyncStatus)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.ErrorMessage)
    .HasColumnType("nvarchar(max)");

            builder.Property(x => x.RawPackageJson)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.RawDetailJson)
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(x => new { x.ProviderId, x.ProviderSku })
                .IsUnique();
        }
    }
}
