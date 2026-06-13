using DTP.Modules.Customer.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Infrastructure.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Domain.Entities.Customer>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Customer> builder)
        {
            builder.ToTable("Customers", "customer");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.FullName)
                .HasMaxLength(200);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(50);

            builder.Property(x => x.AvatarUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.Note)
                .HasMaxLength(1000);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .HasDefaultValue(CustomerStatus.Active)
                .IsRequired();

            builder.HasIndex(x => x.UserId)
                .IsUnique()
                .HasDatabaseName("IX_Customers_UserId");

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("IX_Customers_Email");

            builder.HasMany(x => x.Addresses)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
