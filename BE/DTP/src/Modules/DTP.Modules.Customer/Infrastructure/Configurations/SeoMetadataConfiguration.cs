using DTP.Modules.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Infrastructure.Configurations
{
    public class SeoMetadataConfiguration : IEntityTypeConfiguration<SeoMetadata>
    {
        public class SeoMetadataConfiguration : IEntityTypeConfiguration<SeoMetadata>
        {
            public void Configure(EntityTypeBuilder<SeoMetadata> builder)
            {
                builder.ToTable("SeoMetadata");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.EntityType)
                    .HasMaxLength(100)
                    .IsRequired();

                builder.Property(x => x.RoutePath)
                    .HasMaxLength(500);

                builder.Property(x => x.MetaTitle)
                    .HasMaxLength(255)
                    .IsRequired();

                builder.Property(x => x.MetaDescription)
                    .HasMaxLength(1000);

                builder.Property(x => x.MetaKeywords)
                    .HasMaxLength(1000);

                builder.Property(x => x.CanonicalUrl)
                    .HasMaxLength(1000);

                builder.Property(x => x.OgTitle)
                    .HasMaxLength(255);

                builder.Property(x => x.OgDescription)
                    .HasMaxLength(1000);

                builder.Property(x => x.OgImageUrl)
                    .HasMaxLength(1000);

                builder.Property(x => x.Robots)
                    .HasMaxLength(100)
                    .IsRequired();

                builder.Property(x => x.CreatedAt)
                    .IsRequired();

                builder.HasIndex(x => new
                {
                    x.EntityType,
                    x.EntityId
                });

                builder.HasIndex(x => x.RoutePath);

                builder.HasIndex(x => x.EntityType);
            }
        }
    }
}
