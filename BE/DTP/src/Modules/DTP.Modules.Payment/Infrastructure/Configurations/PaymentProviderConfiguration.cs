using DTP.Modules.Payment.Domain.Constants;
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
    public class PaymentProviderConfiguration : IEntityTypeConfiguration<PaymentProvider>
    {
        public void Configure(EntityTypeBuilder<PaymentProvider> builder)
        {
            builder.ToTable("PaymentProviders", "payment");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.PaymentMethod)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Currency)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.MinAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.MaxAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.LogoUrl)
                .HasMaxLength(500);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasIndex(x => new { x.IsActive, x.SortOrder });

            builder.HasIndex(x => x.IsDefault)
                .IsUnique()
                .HasFilter("[IsDefault] = 1");

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Code = PaymentProviderCodes.Sepay,
                    Name = "SePay QR",
                    PaymentMethod = PaymentMethods.BankQr,
                    IsActive = true,
                    IsDefault = true,
                    SortOrder = 1,
                    MinAmount = (decimal?)1000m,
                    MaxAmount = (decimal?)50000000m,
                    Currency = "VND",
                    LogoUrl = "/images/payment/sepay.png",
                    Description = "Thanh toán bằng mã QR ngân hàng qua SePay.",
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null
                },
                new
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Code = PaymentProviderCodes.VnptEpay,
                    Name = "VNPT ePay",
                    PaymentMethod = PaymentMethods.BankQr,
                    IsActive = true,
                    IsDefault = false,
                    SortOrder = 2,
                    MinAmount = (decimal?)1000m,
                    MaxAmount = (decimal?)50000000m,
                    Currency = "VND",
                    LogoUrl = "/images/payment/vnpt-epay.png",
                    Description = "Thanh toán QR qua VNPT ePay.",
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null
                },
                new
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Code = PaymentProviderCodes.Momo,
                    Name = "MoMo",
                    PaymentMethod = PaymentMethods.EWallet,
                    IsActive = false,
                    IsDefault = false,
                    SortOrder = 3,
                    MinAmount = (decimal?)1000m,
                    MaxAmount = (decimal?)20000000m,
                    Currency = "VND",
                    LogoUrl = "/images/payment/momo.png",
                    Description = "Thanh toán ví điện tử MoMo.",
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null
                },
                new
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Code = PaymentProviderCodes.Vnpay,
                    Name = "VNPAY",
                    PaymentMethod = PaymentMethods.BankQr,
                    IsActive = false,
                    IsDefault = false,
                    SortOrder = 4,
                    MinAmount = (decimal?)1000m,
                    MaxAmount = (decimal?)50000000m,
                    Currency = "VND",
                    LogoUrl = "/images/payment/vnpay.png",
                    Description = "Thanh toán qua VNPAY.",
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null
                }
            );
        }
    }
}
