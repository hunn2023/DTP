using DTP.Modules.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DTP.Modules.Payment.Infrastructure.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("PaymentTransactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderCode)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Currency)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.PaymentProviderId)
                .IsRequired();

            builder.Property(x => x.Method)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.RequestId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ProviderTransactionId)
                .HasMaxLength(200);

            builder.Property(x => x.ProviderPaymentCode)
                .HasMaxLength(200);

            builder.Property(x => x.QrCode)
                .HasMaxLength(4000);

            builder.Property(x => x.QrImageUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.PaymentUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.BankCode)
                .HasMaxLength(50);

            builder.Property(x => x.BankAccountNo)
                .HasMaxLength(100);

            builder.Property(x => x.BankAccountName)
                .HasMaxLength(255);

            builder.Property(x => x.TransferContent)
                .HasMaxLength(500);

            builder.Property(x => x.ProviderResponseCode)
                .HasMaxLength(100);

            builder.Property(x => x.ProviderResponseMessage)
                .HasMaxLength(1000);

            builder.Property(x => x.RawProviderRequest)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.RawProviderResponse)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.RawCallbackData)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.IpAddress)
                .HasMaxLength(100);

            builder.HasIndex(x => x.OrderId)
                 .IsUnique()
                 .HasFilter("[Status] IN (0, 1, 2)");

            builder.HasIndex(x => x.RequestId)
                .IsUnique();

            builder.HasIndex(x => new { x.PaymentProviderId, x.ProviderTransactionId });

            builder.HasIndex(x => x.Status);
        }
    }
}
