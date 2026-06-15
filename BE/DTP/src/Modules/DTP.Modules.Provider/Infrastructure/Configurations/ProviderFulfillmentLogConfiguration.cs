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
    public class ProviderFulfillmentLogConfiguration
     : IEntityTypeConfiguration<ProviderFulfillmentLog>
    {
        public void Configure(EntityTypeBuilder<ProviderFulfillmentLog> builder)
        {
            builder.ToTable("ProviderFulfillmentLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProviderSku)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.RequestBody)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ResponseBody)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.QrCodeUrl)
                .HasMaxLength(2000);

            builder.Property(x => x.ActivationCode)
                .HasMaxLength(1000);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.HasIndex(x => x.OrderItemId);
            builder.HasIndex(x => x.ProviderSku);
        }
    }
}
