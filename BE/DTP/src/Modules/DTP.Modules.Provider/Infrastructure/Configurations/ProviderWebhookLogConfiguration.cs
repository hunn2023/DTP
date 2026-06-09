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
    public class ProviderWebhookLogConfiguration : IEntityTypeConfiguration<ProviderWebhookLog>
    {
        public void Configure(EntityTypeBuilder<ProviderWebhookLog> builder)
        {
            builder.ToTable("Provider_WebhookLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventType)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Payload)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.HasIndex(x => x.ProviderId);

            builder.HasIndex(x => x.ReceivedAt);
        }
    }
}
