using DTP.Modules.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Infrastructure.Configurations
{
    public sealed class EsimPackageCoverageConfiguration : IEntityTypeConfiguration<EsimPackageCoverage>
    {
        public void Configure(EntityTypeBuilder<EsimPackageCoverage> builder)
        {
            builder.ToTable("EsimPackageCoverages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CountryCode)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.CountryName)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasOne(x => x.EsimPackage)
                .WithMany(x => x.Coverages)
                .HasForeignKey(x => x.EsimPackageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.EsimPackageId);

            builder.HasIndex(x => x.CountryId);

            builder.HasIndex(x => new
            {
                x.EsimPackageId,
                x.CountryId
            }).IsUnique();
        }
    }
}
