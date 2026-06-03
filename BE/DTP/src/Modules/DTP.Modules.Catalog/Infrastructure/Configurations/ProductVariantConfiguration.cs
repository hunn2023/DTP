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
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Sku).HasMaxLength(100);

            builder.Property(x => x.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.OriginalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DataAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DataUnit)
                .HasMaxLength(20);

            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => x.Sku);
        }
    }
}
