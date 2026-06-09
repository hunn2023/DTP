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
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.ToTable("ProductAttributes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.Key)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.DisplayName)
                .HasMaxLength(255);

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.Property(x => x.IsVisible)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne<Product>()
                .WithMany(x => x.Attributes)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ProductId);

            builder.HasIndex(x => new { x.ProductId, x.Key })
                .IsUnique();

            builder.HasIndex(x => new { x.ProductId, x.SortOrder });
        }
    }
}
