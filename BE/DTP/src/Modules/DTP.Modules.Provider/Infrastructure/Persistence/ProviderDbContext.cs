using DTP.Modules.Provider.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Persistence
{
    public class ProviderDbContext : DbContext
    {
        public ProviderDbContext(DbContextOptions<ProviderDbContext> options)
            : base(options)
        {
        }

        public DbSet<Domain.Entities.Provider> Providers => Set<Domain.Entities.Provider>();
        public DbSet<ProviderPackageProduct> ProviderPackageProducts => Set<ProviderPackageProduct>();
        public DbSet<ProviderProductMapping> ProviderProductMappings => Set<ProviderProductMapping>();
        public DbSet<ProviderApiLog> ProviderApiLogs => Set<ProviderApiLog>();
        public DbSet<ProviderFulfillmentLog> ProviderFulfillmentLogs => Set<ProviderFulfillmentLog>();

        public DbSet<ProviderOrder> ProviderOrders => Set<ProviderOrder>();
        public DbSet<ProviderOrderItem> ProviderOrderItems => Set<ProviderOrderItem>();
        public DbSet<ProviderRedeem> ProviderRedeems => Set<ProviderRedeem>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProviderDbContext).Assembly);
        }
    }
}
