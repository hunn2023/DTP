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
    public class ProviderProductMappingConfiguration
     : IEntityTypeConfiguration<ProviderProductMapping>
    {
        public void Configure(EntityTypeBuilder<ProviderProductMapping> builder)
        {
            builder.ToTable("ProviderProductMappings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProviderSku)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ProviderProductId)
                .HasMaxLength(100);

            builder.Property(x => x.MappingStatus)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => new { x.ProviderId, x.ProviderSku })
                .IsUnique();

            builder.HasIndex(x => x.EsimPackageId)
                .IsUnique(false);
        }
    }
}
