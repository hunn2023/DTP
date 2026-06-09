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
    public class ProviderProductMappingConfiguration : IEntityTypeConfiguration<ProviderProductMapping>
    {
        public void Configure(EntityTypeBuilder<ProviderProductMapping> builder)
        {
            builder.ToTable("Provider_ProductMappings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProviderProductCode)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.ProviderProductName)
                .HasMaxLength(500);

            builder.Property(x => x.ProviderCostPrice)
                .HasPrecision(18, 4);

            builder.Property(x => x.CurrencyCode)
                .HasMaxLength(10);

            builder.HasIndex(x => new
            {
                x.ProviderId,
                x.ProductVariantId
            }).IsUnique();
        }
    }
}
