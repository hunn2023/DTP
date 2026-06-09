using DTP.Modules.Delivery.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DTP.Modules.Delivery.Infrastructure.Configurations
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<Domain.Entities.Delivery>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Delivery> builder)
        {
            builder.ToTable("Deliveries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderCode)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.CustomerName)
                .HasMaxLength(255);

            builder.Property(x => x.CustomerEmail)
                .HasMaxLength(255);

            builder.Property(x => x.IpAddress)
                .HasMaxLength(100);

            builder.Property(x => x.LastError)
                .HasMaxLength(2000);

            builder.Property(x => x.Note)
                .HasMaxLength(2000);

            builder.Property(x => x.DeliveryType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .HasDefaultValue(DeliveryStatus.Pending)
                .IsRequired();

            builder.HasIndex(x => x.OrderId)
                .IsUnique();

            builder.HasIndex(x => x.OrderCode);

            builder.HasIndex(x => x.Status);

            builder.HasMany(x => x.Items)
                .WithOne(x => x.Delivery)
                .HasForeignKey(x => x.DeliveryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Histories)
                .WithOne(x => x.Delivery)
                .HasForeignKey(x => x.DeliveryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(x => x.Histories)
                .UsePropertyAccessMode(PropertyAccessMode.Field);


            builder.Property(x => x.EmailSent)
                .HasDefaultValue(false);

            builder.Property(x => x.EmailError)
                .HasMaxLength(2000);

            builder.Property(x => x.EmailSentAt);
        }
    }
}
