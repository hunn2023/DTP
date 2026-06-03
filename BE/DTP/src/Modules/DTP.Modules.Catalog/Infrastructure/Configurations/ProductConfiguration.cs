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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Code)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Slug)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.CategoryId)
                .IsRequired();


            builder.Property(p => p.ShortDescription)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(p => p.Description)
                .IsRequired(false);

            builder.Property(p => p.ThumbnailUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(p => p.SortOrder)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired();

            builder.HasMany(p => p.Variants)
                .WithOne()
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Images)
                .WithOne()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Attributes)
                .WithOne()
                .HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
