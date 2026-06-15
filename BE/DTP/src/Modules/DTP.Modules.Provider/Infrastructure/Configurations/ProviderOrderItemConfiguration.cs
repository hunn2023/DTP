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
    public class ProviderOrderItemConfiguration : IEntityTypeConfiguration<ProviderOrderItem>
    {
        public void Configure(EntityTypeBuilder<ProviderOrderItem> builder)
        {
            builder.ToTable("ProviderOrderItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Sku)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ProductName)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.RawSerialsJson)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.HasIndex(x => x.ProviderOrderId);

            builder.HasIndex(x => x.DtpOrderItemId);

            builder.HasIndex(x => new
            {
                x.ProviderOrderId,
                x.Sku
            });

            builder.HasMany(x => x.Redeems)
                .WithOne(x => x.ProviderOrderItem)
                .HasForeignKey(x => x.ProviderOrderItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
