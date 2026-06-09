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
    public class ContentFaqConfiguration : IEntityTypeConfiguration<ContentFaq>
    {
        public void Configure(EntityTypeBuilder<ContentFaq> builder)
        {
            builder.ToTable("ContentFaqs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Question)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.Answer)
                .IsRequired();

            builder.Property(x => x.CategoryCode)
                .HasMaxLength(100);

            builder.HasIndex(x => x.CategoryCode);
            builder.HasIndex(x => x.IsActive);
        }
    }
}
