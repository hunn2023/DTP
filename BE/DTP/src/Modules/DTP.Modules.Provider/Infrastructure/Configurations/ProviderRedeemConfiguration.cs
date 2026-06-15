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
    public class ProviderRedeemConfiguration : IEntityTypeConfiguration<ProviderRedeem>
    {
        public void Configure(EntityTypeBuilder<ProviderRedeem> builder)
        {
            builder.ToTable("ProviderRedeems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Serial)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Sku)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.PackageName)
                .HasMaxLength(500);

            builder.Property(x => x.Model)
                .HasMaxLength(255);

            builder.Property(x => x.Iccid)
                .HasMaxLength(100);

            builder.Property(x => x.Imsi)
                .HasMaxLength(100);

            builder.Property(x => x.ActivationCode)
                .HasMaxLength(2000);

            builder.Property(x => x.QrCodeUrl)
                .HasMaxLength(2000);

            builder.Property(x => x.ShortUrlApple)
                .HasMaxLength(2000);

            builder.Property(x => x.ShortUrlAndroid)
                .HasMaxLength(2000);

            builder.Property(x => x.Apn)
                .HasMaxLength(255);

            builder.Property(x => x.PolicyNumber)
                .HasMaxLength(255);

            builder.Property(x => x.PolicyUrl)
                .HasMaxLength(2000);

            builder.Property(x => x.PolicyCertificate)
                .HasMaxLength(2000);

            builder.Property(x => x.RawRedeemResponseJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.RawRedeemInfoJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.HasIndex(x => x.Serial)
                .IsUnique();

            builder.HasIndex(x => x.DtpOrderId);

            builder.HasIndex(x => x.DtpOrderItemId);

            builder.HasIndex(x => new
            {
                x.Status,
                x.EmailSent
            });
        }
    }
}
