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
            builder.ToTable("Provider_Orders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderCode)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ProviderOrderCode)
                .HasMaxLength(255);

            builder.Property(x => x.ErrorCode)
                .HasMaxLength(100);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.HasIndex(x => x.OrderId)
                .IsUnique();

            builder.HasMany(x => x.Items)
                .WithOne(x => x.ProviderOrder)
                .HasForeignKey(x => x.ProviderOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
