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
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("PaymentTransactions", "payment");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderCode)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.TransactionCode)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ProviderTransactionCode)
                .HasMaxLength(100);

            builder.Property(x => x.ProviderCode)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CurrencyCode)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.PaymentUrl)
                .HasMaxLength(2000);

            builder.Property(x => x.QrCodeUrl)
                .HasMaxLength(2000);

            builder.Property(x => x.QrContent)
                .HasMaxLength(4000);

            builder.Property(x => x.FailureReason)
                .HasMaxLength(1000);

            builder.Property(x => x.RawRequest)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.RawResponse)
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(x => x.OrderId);

            builder.HasIndex(x => x.OrderCode);

            builder.HasIndex(x => x.TransactionCode)
                .IsUnique();

            builder.HasIndex(x => x.ProviderTransactionCode);
        }
    }
}
