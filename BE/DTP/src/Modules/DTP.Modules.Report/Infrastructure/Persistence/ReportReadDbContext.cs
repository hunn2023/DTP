using DTP.Modules.Report.Application.Abstractions.Persistence;
using DTP.Modules.Report.Application.Abstractions.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Infrastructure.Persistence
{

    public class ReportReadDbContext : DbContext, IReportReadDbContext
    {
        public ReportReadDbContext(DbContextOptions<ReportReadDbContext> options)
            : base(options)
        {
        }

        public DbSet<ReportOrderReadModel> Orders => Set<ReportOrderReadModel>();
        public DbSet<ReportOrderItemReadModel> OrderItems => Set<ReportOrderItemReadModel>();
        public DbSet<ReportPaymentReadModel> Payments => Set<ReportPaymentReadModel>();
        public DbSet<ReportProductReadModel> Products => Set<ReportProductReadModel>();
        public DbSet<ReportCategoryReadModel> Categories => Set<ReportCategoryReadModel>();
        public DbSet<ReportCustomerReadModel> Customers => Set<ReportCustomerReadModel>();
        public DbSet<ReportProviderReadModel> Providers => Set<ReportProviderReadModel>();

        public IQueryable<ReportOrderReadModel> OrderQuery => Orders.AsNoTracking();
        public IQueryable<ReportOrderItemReadModel> OrderItemQuery => OrderItems.AsNoTracking();
        public IQueryable<ReportPaymentReadModel> PaymentQuery => Payments.AsNoTracking();
        public IQueryable<ReportProductReadModel> ProductQuery => Products.AsNoTracking();
        public IQueryable<ReportCategoryReadModel> CategoryQuery => Categories.AsNoTracking();
        public IQueryable<ReportCustomerReadModel> CustomerQuery => Customers.AsNoTracking();
        public IQueryable<ReportProviderReadModel> ProviderQuery => Providers.AsNoTracking();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportOrderReadModel>(builder =>
            {
                builder.ToTable("Orders", "ordering");
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ReportOrderItemReadModel>(builder =>
            {
                builder.ToTable("OrderItems", "ordering");
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ReportPaymentReadModel>(builder =>
            {
                builder.ToTable("Payments", "payment");
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ReportProductReadModel>(builder =>
            {
                builder.ToTable("Products", "catalog");
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ReportCategoryReadModel>(builder =>
            {
                builder.ToTable("Categories", "catalog");
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ReportCustomerReadModel>(builder =>
            {
                builder.ToTable("Customers", "customer");
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ReportProviderReadModel>(builder =>
            {
                builder.ToTable("Providers", "provider");
                builder.HasKey(x => x.Id);
            });
        }
    }
}
