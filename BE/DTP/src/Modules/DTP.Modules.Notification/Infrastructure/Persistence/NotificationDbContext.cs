using DTP.Modules.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Infrastructure.Persistence
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options)
        {
        }

        public DbSet<NotificationMessage> NotificationMessages => Set<NotificationMessage>();

        public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();

        public DbSet<NotificationDeliveryLog> NotificationDeliveryLogs => Set<NotificationDeliveryLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureNotificationMessage(modelBuilder);
            ConfigureNotificationTemplate(modelBuilder);
            ConfigureNotificationDeliveryLog(modelBuilder);
        }

        private static void ConfigureNotificationMessage(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificationMessage>(builder =>
            {
                builder.ToTable("NotificationMessages");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Email)
                    .HasMaxLength(255);

                builder.Property(x => x.Title)
                    .HasMaxLength(500)
                    .IsRequired();

                builder.Property(x => x.Content)
                    .IsRequired();

                builder.Property(x => x.ReferenceType)
                    .HasMaxLength(100);

                builder.Property(x => x.ErrorMessage)
                    .HasMaxLength(2000);

                builder.Property(x => x.Type)
                    .HasConversion<int>();

                builder.Property(x => x.Channel)
                    .HasConversion<int>();

                builder.Property(x => x.Status)
                    .HasConversion<int>();

                builder.HasIndex(x => x.UserId);

                builder.HasIndex(x => x.Email);

                builder.HasIndex(x => x.Status);

                builder.HasIndex(x => new { x.ReferenceType, x.ReferenceId });
            });
        }

        private static void ConfigureNotificationTemplate(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificationTemplate>(builder =>
            {
                builder.ToTable("NotificationTemplates");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Code)
                    .HasMaxLength(100)
                    .IsRequired();

                builder.Property(x => x.TitleTemplate)
                    .HasMaxLength(500)
                    .IsRequired();

                builder.Property(x => x.ContentTemplate)
                    .IsRequired();

                builder.Property(x => x.Type)
                    .HasConversion<int>();

                builder.Property(x => x.Channel)
                    .HasConversion<int>();

                builder.HasIndex(x => x.Code)
                    .IsUnique();
            });
        }

        private static void ConfigureNotificationDeliveryLog(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificationDeliveryLog>(builder =>
            {
                builder.ToTable("NotificationDeliveryLogs");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Provider)
                    .HasMaxLength(100);

                builder.Property(x => x.RequestPayload);

                builder.Property(x => x.ResponsePayload);

                builder.Property(x => x.ErrorMessage)
                    .HasMaxLength(2000);

                builder.Property(x => x.Channel)
                    .HasConversion<int>();

                builder.Property(x => x.Status)
                    .HasConversion<int>();

                builder.HasIndex(x => x.NotificationMessageId);
            });
        }
    }
}
