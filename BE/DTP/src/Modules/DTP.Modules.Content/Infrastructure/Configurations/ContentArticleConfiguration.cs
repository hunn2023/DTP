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
    public class ContentArticleConfiguration : IEntityTypeConfiguration<ContentArticle>
    {
        public void Configure(EntityTypeBuilder<ContentArticle> builder)
        {
            builder.ToTable("ContentArticles");

            builder.HasKey(x => x.Id);

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

            builder.Property(x => x.AuthorName)
                .HasMaxLength(255);

            builder.Property(x => x.CategoryCode)
                .HasMaxLength(100);

            builder.Property(x => x.Tags)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.Slug)
                .IsUnique();

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.CategoryCode);

            builder.HasIndex(x => x.IsFeatured);
        }
    }
}
