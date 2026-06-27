using DTP.Modules.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Configurations
{
    public class PaymentCallbackConfiguration : IEntityTypeConfiguration<PaymentCallback>
    {
        public void Configure(EntityTypeBuilder<PaymentCallback> builder)
        {
            builder.ToTable("PaymentCallbacks", "payment");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PaymentProviderId)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.OrderCode)
                .HasMaxLength(50);

            builder.Property(x => x.TransactionCode)
                .HasMaxLength(100);

            builder.Property(x => x.ProviderTransactionCode)
                .HasMaxLength(100);

            builder.Property(x => x.Signature)
                .HasMaxLength(1000);

            builder.Property(x => x.RequestBody)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.HasIndex(x => x.PaymentTransactionId);

            builder.HasIndex(x => x.OrderCode);

            builder.HasIndex(x => x.TransactionCode);
        }
    }
}
