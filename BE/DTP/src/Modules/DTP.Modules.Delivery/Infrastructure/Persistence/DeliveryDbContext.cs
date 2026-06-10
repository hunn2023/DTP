using DTP.Modules.Delivery.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Delivery.Infrastructure.Persistence
{
    public class DeliveryDbContext : DbContext
    {
        public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Domain.Entities.Delivery> Deliveries => Set<Domain.Entities.Delivery>();

        public DbSet<DeliveryItem> DeliveryItems => Set<DeliveryItem>();

        public DbSet<DeliveryStatusHistory> DeliveryStatusHistories => Set<DeliveryStatusHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeliveryDbContext).Assembly);
        }
    }
}
