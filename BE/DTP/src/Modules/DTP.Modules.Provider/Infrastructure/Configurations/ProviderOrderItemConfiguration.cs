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
    public class ProviderOrderItemConfiguration : IEntityTypeConfiguration<ProviderOrderItem>
    {
        public void Configure(EntityTypeBuilder<ProviderOrderItem> builder)
        {
            builder.ToTable("Provider_OrderItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProviderProductCode)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Iccid)
                .HasMaxLength(100);

            builder.Property(x => x.Msisdn)
                .HasMaxLength(100);

            builder.Property(x => x.QrCodeUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.QrCodeText)
                .HasMaxLength(4000);

            builder.Property(x => x.ActivationCode)
                .HasMaxLength(1000);

            builder.Property(x => x.Serial)
                .HasMaxLength(255);

            builder.Property(x => x.PinCode)
                .HasMaxLength(255);

            builder.HasIndex(x => x.OrderItemId);
        }
    }
}
