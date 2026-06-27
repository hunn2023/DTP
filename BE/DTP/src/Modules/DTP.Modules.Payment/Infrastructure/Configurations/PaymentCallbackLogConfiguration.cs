using DTP.Modules.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DTP.Modules.Payment.Infrastructure.Configurations
{
    public class PaymentCallbackLogConfiguration : IEntityTypeConfiguration<PaymentCallbackLog>
    {
        public void Configure(EntityTypeBuilder<PaymentCallbackLog> builder)
        {
            builder.ToTable("PaymentCallbackLogs");

            builder.HasKey(x => x.Id);


            builder.Property(x => x.PaymentProviderId)
                .IsRequired();

            builder.Property(x => x.RequestId)
                .HasMaxLength(100);

            builder.Property(x => x.ProviderTransactionId)
                .HasMaxLength(200);

            builder.Property(x => x.RawBody)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.Signature)
                .HasMaxLength(1000);

            builder.Property(x => x.IpAddress)
                .HasMaxLength(100);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.PaymentTransactionId);

            builder.HasIndex(x => x.RequestId);

            builder.HasIndex(x => x.ProviderTransactionId);

            builder.HasIndex(x => x.Status);
        }
    }
}
