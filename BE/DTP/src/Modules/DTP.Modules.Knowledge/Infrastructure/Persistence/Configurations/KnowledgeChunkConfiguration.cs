using DTP.Modules.Knowledge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Infrastructure.Persistence.Configurations
{
    public class KnowledgeChunkConfiguration : IEntityTypeConfiguration<KnowledgeChunk>
    {
        public void Configure(EntityTypeBuilder<KnowledgeChunk> builder)
        {
            builder.ToTable("KnowledgeChunks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SourceType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.SourceId)
                .IsRequired();

            builder.Property(x => x.ChunkIndex)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.Slug)
                .HasMaxLength(500);

            builder.Property(x => x.SourceUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.Content)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.ContentHash)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(x => x.EmbeddingJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.EmbeddingModel)
                .HasMaxLength(100);

            builder.Property(x => x.LanguageCode)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.SourceType,
                x.SourceId,
                x.ChunkIndex
            }).IsUnique();

            builder.HasIndex(x => x.IsActive);

            builder.HasIndex(x => x.SourceType);
        }
    }
}
