using DTP.Modules.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Infrastructure.Configurations
{
    public class PendingRegistrationConfiguration : IEntityTypeConfiguration<PendingRegistration>
    {
        public void Configure(EntityTypeBuilder<PendingRegistration> builder)
        {
            builder.ToTable("PendingRegistrations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.FullName)
                .HasMaxLength(255);

            builder.Property(x => x.Phone)
                .HasMaxLength(50);

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.OtpCodeHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.OtpExpiredAt)
                .IsRequired();

            //builder.Property(x => x.IsUsed)
            //    .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            //builder.Property(x => x.UsedAt);

            builder.HasIndex(x => x.Email);

            //builder.HasIndex(x => x.OtpCode);
        }
    }
}
