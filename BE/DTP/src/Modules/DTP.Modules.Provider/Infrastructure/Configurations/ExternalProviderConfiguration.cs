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
    public class ExternalProviderConfiguration : IEntityTypeConfiguration<ExternalProvider>
    {
        public void Configure(EntityTypeBuilder<ExternalProvider> builder)
        {
            builder.ToTable("Provider_ExternalProviders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.ApiBaseUrl)
                .HasMaxLength(500);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasMany(x => x.Credentials)
                .WithOne(x => x.Provider)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ProductMappings)
                .WithOne(x => x.Provider)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
