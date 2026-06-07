
using DTP.Modules.Customer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Infrastructure.Persistence
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Domain.Entities.Customer> Customers => Set<Domain.Entities.Customer>();

        public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerDbContext).Assembly);
        }
    }
}
