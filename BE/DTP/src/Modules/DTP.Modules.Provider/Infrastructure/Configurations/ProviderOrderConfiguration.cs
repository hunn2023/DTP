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
    public class ProviderOrderConfiguration : IEntityTypeConfiguration<ProviderOrder>
    {
        public void Configure(EntityTypeBuilder<ProviderOrder> builder)
        {
            builder.ToTable("ProviderOrders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProviderOrderPublicId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.RawCreateResponseJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.RawConfirmResponseJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.HasIndex(x => x.DtpOrderId);

            builder.HasIndex(x => x.ProviderOrderPublicId)
                .IsUnique();

            builder.HasMany(x => x.Items)
                .WithOne(x => x.ProviderOrder)
                .HasForeignKey(x => x.ProviderOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
