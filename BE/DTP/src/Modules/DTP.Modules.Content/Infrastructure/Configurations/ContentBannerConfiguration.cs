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
    public class ContentBannerConfiguration : IEntityTypeConfiguration<ContentBanner>
    {
        public void Configure(EntityTypeBuilder<ContentBanner> builder)
        {
            builder.ToTable("ContentBanners");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.MobileImageUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.LinkUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.Position);
            builder.HasIndex(x => x.IsActive);
        }
    }
}
