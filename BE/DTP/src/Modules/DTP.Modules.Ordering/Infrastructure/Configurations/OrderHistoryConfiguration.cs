using DTP.Modules.Ordering.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Infrastructure.Configurations
{
    public class OrderHistoryConfiguration : IEntityTypeConfiguration<OrderHistory>
    {
        public void Configure(EntityTypeBuilder<OrderHistory> builder)
        {
            builder.ToTable("OrderHistories", "ordering");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FromStatus)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.ToStatus)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Note)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.OrderId);
        }
    }
}
