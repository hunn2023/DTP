using DTP.Modules.Delivery.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Infrastructure.Persistence
{
    public class DeliveryDbContext : DbContext
    {
        public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options)
            : base(options)
        {
        }

        public DbSet<EsimProfile> EsimProfiles => Set<EsimProfile>();

        public DbSet<DigitalDelivery> DigitalDeliveries => Set<DigitalDelivery>();

        public DbSet<DeliveryLog> DeliveryLogs => Set<DeliveryLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeliveryDbContext).Assembly);
        }
    }
}
