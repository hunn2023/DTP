using DTP.Modules.Ordering.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DTP.Modules.Ordering.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "ordering");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderCode)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.OrderCode)
                .IsUnique();

            builder.Property(x => x.CustomerEmail)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.CustomerName)
                .HasMaxLength(255);

            builder.Property(x => x.CustomerPhone)
                .HasMaxLength(50);

            builder.Property(x => x.CurrencyCode)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.SubtotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.PaymentStatus)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.PaymentTransactionCode)
                .HasMaxLength(100);

            builder.Property(x => x.Note)
                .HasMaxLength(1000);

            builder.HasMany(x => x.Items)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Histories)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(x => x.Histories)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(x => x.IpAddress)
                .HasMaxLength(100);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(1000);

            builder.Property(x => x.ReferrerUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.CheckoutSource)
                .HasMaxLength(100);
        }
    }
}
