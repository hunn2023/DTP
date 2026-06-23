using DTP.Modules.Report.Application.Abstractions.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Abstractions.Persistence
{
    public interface IReportReadDbContext
    {
        DbSet<ReportOrderReadModel> Orders { get; }
        DbSet<ReportOrderItemReadModel> OrderItems { get; }
        DbSet<ReportPaymentReadModel> PaymentTransactions { get; }
        DbSet<ReportProductReadModel> Products { get; }
        DbSet<ReportCategoryReadModel> Categories { get; }
        DbSet<ReportCustomerReadModel> Customers { get; }
        DbSet<ReportProviderReadModel> Providers { get; }

        DbSet<ReportUserRoleReadModel> UserRoles { get; }
        DbSet<ReportRoleReadModel> Roles { get; }

        IQueryable<ReportUserRoleReadModel> UserRoleQuery { get; }
        IQueryable<ReportRoleReadModel> RoleQuery { get; }

        DbSet<ReportEsimPackageReadModel> EsimPackages { get; }

        IQueryable<ReportEsimPackageReadModel> EsimPackageQuery { get; }


        DbSet<ReportCountryReadModel> Countries { get; }

        IQueryable<ReportCountryReadModel> CountryQuery { get; }


        IQueryable<ReportOrderReadModel> OrderQuery { get; }
        IQueryable<ReportOrderItemReadModel> OrderItemQuery { get; }
        IQueryable<ReportPaymentReadModel> PaymentTransactionQuery { get; }
        IQueryable<ReportProductReadModel> ProductQuery { get; }
        IQueryable<ReportCategoryReadModel> CategoryQuery { get; }
        IQueryable<ReportCustomerReadModel> CustomerQuery { get; }
        IQueryable<ReportProviderReadModel> ProviderQuery { get; }
    }
}
