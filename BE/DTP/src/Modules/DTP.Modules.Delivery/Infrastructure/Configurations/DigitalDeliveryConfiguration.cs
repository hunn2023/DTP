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
    public class DigitalDeliveryConfiguration : IEntityTypeConfiguration<DigitalDelivery>
    {
        public void Configure(EntityTypeBuilder<DigitalDelivery> builder)
        {
            builder.ToTable("DigitalDeliveries", "delivery");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RecipientEmail)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.FailedReason)
                .HasMaxLength(2000);

            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.OrderItemId);
            builder.HasIndex(x => x.EsimProfileId);
            builder.HasIndex(x => x.Status);
        }
    }
}
