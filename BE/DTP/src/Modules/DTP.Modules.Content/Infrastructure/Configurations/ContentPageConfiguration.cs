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
    public class ContentPageConfiguration : IEntityTypeConfiguration<ContentPage>
    {
        public void Configure(EntityTypeBuilder<ContentPage> builder)
        {
            builder.ToTable("ContentPages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Slug)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Summary)
                .HasMaxLength(1000);

            builder.Property(x => x.Content)
                .IsRequired();

            builder.Property(x => x.ThumbnailUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasIndex(x => x.Slug)
                .IsUnique();

            builder.HasIndex(x => x.Status);
        }
    }
}
