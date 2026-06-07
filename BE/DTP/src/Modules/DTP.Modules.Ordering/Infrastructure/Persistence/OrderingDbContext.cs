using DTP.Modules.Ordering.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Ordering.Infrastructure.Persistence
{
    public class OrderingDbContext : DbContext
    {
        public OrderingDbContext(DbContextOptions<OrderingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<OrderHistory> OrderHistories => Set<OrderHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
        }
    }
}
