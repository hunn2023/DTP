using DTP.Modules.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Infrastructure.Configurations
{
    public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
    {
        public void Configure(EntityTypeBuilder<CustomerAddress> builder)
        {
            builder.ToTable("CustomerAddresses", "customer");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReceiverName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.AddressLine)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.Ward)
                .HasMaxLength(100);

            builder.Property(x => x.District)
                .HasMaxLength(100);

            builder.Property(x => x.City)
                .HasMaxLength(100);

            builder.Property(x => x.CountryCode)
                .HasMaxLength(10)
                .HasDefaultValue("VN")
                .IsRequired();

            builder.Property(x => x.IsDefault)
                .HasDefaultValue(false)
                .IsRequired();

            builder.HasIndex(x => x.CustomerId)
                .HasDatabaseName("IX_CustomerAddresses_CustomerId");

            builder.HasIndex(x => new { x.CustomerId, x.IsDefault })
                .HasDatabaseName("IX_CustomerAddresses_CustomerId_IsDefault");
        }
    }
}
