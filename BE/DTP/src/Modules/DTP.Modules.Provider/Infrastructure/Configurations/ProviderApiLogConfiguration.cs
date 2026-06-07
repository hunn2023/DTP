using DTP.Modules.Provider.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Configurations
{
    public class ProviderApiLogConfiguration : IEntityTypeConfiguration<ProviderApiLog>
    {
        public void Configure(EntityTypeBuilder<ProviderApiLog> builder)
        {
            builder.ToTable("Provider_ApiLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Endpoint)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.Method)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.RequestBody)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ResponseBody)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.StatusCode);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.Property(x => x.RequestedAt)
                .IsRequired();

            builder.Property(x => x.DurationMs);

            builder.HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ProviderId);

            builder.HasIndex(x => x.LogType);

            builder.HasIndex(x => x.IsSuccess);

            builder.HasIndex(x => x.RequestedAt);
        }
    }
}
