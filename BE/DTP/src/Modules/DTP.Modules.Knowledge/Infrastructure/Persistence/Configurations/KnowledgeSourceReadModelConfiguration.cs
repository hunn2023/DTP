using DTP.Modules.Knowledge.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Infrastructure.Persistence.Configurations
{
    public class ProductKnowledgeReadModelConfiguration : IEntityTypeConfiguration<ProductKnowledgeReadModel>
    {
        public void Configure(EntityTypeBuilder<ProductKnowledgeReadModel> builder)
        {
            builder.ToTable("Products", t => t.ExcludeFromMigrations());

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(500);
            builder.Property(x => x.Slug).HasMaxLength(500);
            builder.Property(x => x.ShortDescription).HasColumnType("nvarchar(max)");
            builder.Property(x => x.Description).HasColumnType("nvarchar(max)");
            builder.Property(x => x.LocationText).HasMaxLength(500);
        }
    }

    public class ProductFaqKnowledgeReadModelConfiguration : IEntityTypeConfiguration<ProductFaqKnowledgeReadModel>
    {
        public void Configure(EntityTypeBuilder<ProductFaqKnowledgeReadModel> builder)
        {
            builder.ToTable("ProductFaqs", t => t.ExcludeFromMigrations());

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Question).HasColumnType("nvarchar(max)");
            builder.Property(x => x.Answer).HasColumnType("nvarchar(max)");
        }
    }

    public class ContentKnowledgeReadModelConfiguration : IEntityTypeConfiguration<ContentKnowledgeReadModel>
    {
        public void Configure(EntityTypeBuilder<ContentKnowledgeReadModel> builder)
        {
            builder.ToTable("Contents", t => t.ExcludeFromMigrations());

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(500);
            builder.Property(x => x.Slug).HasMaxLength(500);
            builder.Property(x => x.Summary).HasColumnType("nvarchar(max)");
            builder.Property(x => x.Body).HasColumnType("nvarchar(max)");
        }
    }

    public class ContentFaqKnowledgeReadModelConfiguration : IEntityTypeConfiguration<ContentFaqKnowledgeReadModel>
    {
        public void Configure(EntityTypeBuilder<ContentFaqKnowledgeReadModel> builder)
        {
            builder.ToTable("ContentFaqs", t => t.ExcludeFromMigrations());

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Question).HasColumnType("nvarchar(max)");
            builder.Property(x => x.Answer).HasColumnType("nvarchar(max)");
        }
    }

    public class ProductContentKnowledgeReadModelConfiguration
    : IEntityTypeConfiguration<ProductContentKnowledgeReadModel>
    {
        public void Configure(EntityTypeBuilder<ProductContentKnowledgeReadModel> builder)
        {
            builder.ToTable("ProductContents", t => t.ExcludeFromMigrations());

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(500);

            builder.Property(x => x.Slug)
                .HasMaxLength(500);

            builder.Property(x => x.Summary)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.BodyHtml)
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(x => x.ProductId);
        }
    }
}
