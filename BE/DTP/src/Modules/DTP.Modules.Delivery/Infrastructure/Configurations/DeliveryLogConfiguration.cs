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
    public class DeliveryLogConfiguration : IEntityTypeConfiguration<DeliveryLog>
    {
        public void Configure(EntityTypeBuilder<DeliveryLog> builder)
        {
            builder.ToTable("DeliveryLogs", "delivery");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Action)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Message)
                .HasMaxLength(2000);

            builder.Property(x => x.RawData)
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.OrderItemId);
            builder.HasIndex(x => x.Status);
        }
    }
}
