using DTP.Modules.Audit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Infrastructure.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs", "audit");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Module)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.ActionType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.UserName)
                .HasMaxLength(256);

            builder.Property(x => x.EntityName)
                .HasMaxLength(150);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.OldValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.NewValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.IpAddress)
                .HasMaxLength(100);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(1000);

            builder.Property(x => x.RequestPath)
                .HasMaxLength(500);

            builder.Property(x => x.RequestMethod)
                .HasMaxLength(20);

            builder.Property(x => x.CorrelationId)
                .HasMaxLength(100);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.HasIndex(x => x.Module);

            builder.HasIndex(x => x.ActionType);

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.EntityName);

            builder.HasIndex(x => x.EntityId);

            builder.HasIndex(x => x.CreatedAt);

            builder.HasIndex(x => new
            {
                x.Module,
                x.EntityName,
                x.EntityId
            });
        }
    }
}
