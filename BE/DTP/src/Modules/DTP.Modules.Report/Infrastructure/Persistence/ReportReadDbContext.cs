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
        public DbSet<ReportPaymentReadModel> PaymentTransactions => Set<ReportPaymentReadModel>();
        public DbSet<ReportProductReadModel> Products => Set<ReportProductReadModel>();
        public DbSet<ReportCategoryReadModel> Categories => Set<ReportCategoryReadModel>();
        public DbSet<ReportCustomerReadModel> Customers => Set<ReportCustomerReadModel>();
        public DbSet<ReportProviderReadModel> Providers => Set<ReportProviderReadModel>();

        public DbSet<ReportUserRoleReadModel> UserRoles => Set<ReportUserRoleReadModel>();
        public DbSet<ReportRoleReadModel> Roles => Set<ReportRoleReadModel>();

        public DbSet<ReportEsimPackageReadModel> EsimPackages
    => Set<ReportEsimPackageReadModel>();

        public IQueryable<ReportOrderReadModel> OrderQuery => Orders.AsNoTracking();
        public IQueryable<ReportOrderItemReadModel> OrderItemQuery => OrderItems.AsNoTracking();
        public IQueryable<ReportPaymentReadModel> PaymentTransactionQuery => PaymentTransactions.AsNoTracking();
        public IQueryable<ReportProductReadModel> ProductQuery => Products.AsNoTracking();
        public IQueryable<ReportCategoryReadModel> CategoryQuery => Categories.AsNoTracking();
        public IQueryable<ReportCustomerReadModel> CustomerQuery => Customers.AsNoTracking();
        public IQueryable<ReportProviderReadModel> ProviderQuery => Providers.AsNoTracking();

        public IQueryable<ReportUserRoleReadModel> UserRoleQuery
    => UserRoles.AsNoTracking();

        public IQueryable<ReportRoleReadModel> RoleQuery
            => Roles.AsNoTracking();

        public IQueryable<ReportEsimPackageReadModel> EsimPackageQuery
    => EsimPackages.AsNoTracking();

        public DbSet<ReportCountryReadModel> Countries => Set<ReportCountryReadModel>();

        public IQueryable<ReportCountryReadModel> CountryQuery =>
            Countries.AsNoTracking();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportOrderReadModel>(builder =>
            {
                builder.ToTable("Orders", "ordering", t => t.ExcludeFromMigrations());
                builder.HasKey(x => x.Id);

                builder.Property(x => x.TotalAmount)
                    .HasColumnName("TotalAmount")
                    .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<ReportOrderItemReadModel>(builder =>
            {
                builder.ToTable("OrderItems", "ordering", t => t.ExcludeFromMigrations());
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ReportPaymentReadModel>(builder =>
            {
                builder.ToTable("PaymentTransactions", "dbo", t => t.ExcludeFromMigrations());
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ReportProductReadModel>(builder =>
            {
                builder.ToTable("Products", "dbo", t => t.ExcludeFromMigrations());
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ReportCategoryReadModel>(builder =>
            {
                builder.ToTable("Category", "dbo", t => t.ExcludeFromMigrations());
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ReportCountryReadModel>(builder =>
            {
                builder.ToTable("Country", "dbo", t => t.ExcludeFromMigrations());
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ReportCustomerReadModel>(builder =>
            {
                builder.ToTable("Users", "dbo", t => t.ExcludeFromMigrations());
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ReportEsimPackageReadModel>(builder =>
            {
                builder.ToTable("EsimPackages", "dbo", t => t.ExcludeFromMigrations());
                builder.HasKey(x => x.Id);

                builder.Property(x => x.ProductId)
                    .HasColumnName("ProductId");

                builder.Property(x => x.ProviderId)
                    .HasColumnName("ProviderId");
            });


            modelBuilder.Entity<ReportUserRoleReadModel>(builder =>
            {
                builder.ToTable("UserRoles", "dbo", t => t.ExcludeFromMigrations());

                builder.HasKey(x => new
                {
                    x.UserId,
                    x.RoleId
                });
            });

            modelBuilder.Entity<ReportRoleReadModel>(builder =>
            {
                builder.ToTable("Roles", "dbo", t => t.ExcludeFromMigrations());
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Name)
                    .HasMaxLength(100);
            });


            modelBuilder.Entity<ReportProviderReadModel>(builder =>
            {
                builder.ToTable("Providers", "dbo", t => t.ExcludeFromMigrations());
                builder.HasKey(x => x.Id);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
