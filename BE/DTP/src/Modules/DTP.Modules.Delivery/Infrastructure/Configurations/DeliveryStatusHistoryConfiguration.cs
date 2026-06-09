using DTP.Modules.Delivery.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Infrastructure.Configurations
{
    public class DeliveryStatusHistoryConfiguration : IEntityTypeConfiguration<DeliveryStatusHistory>
    {
        public void Configure(EntityTypeBuilder<DeliveryStatusHistory> builder)
        {
            builder.ToTable("DeliveryStatusHistories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Message)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.Detail)
                .HasMaxLength(2000);

            builder.HasIndex(x => x.DeliveryId);

            builder.HasIndex(x => x.Status);
        }
    }
}
