using DTP.Modules.Report.Application.DTOs;
using DTP.Modules.Report.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Abstractions.Services
{
    public interface IReportService
    {
        Task<DashboardReportDto> GetDashboardReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default);

        Task<SalesReportDto> GetSalesReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default);

        Task<OrderReportDto> GetOrderReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default);

        Task<PaymentReportDto> GetPaymentReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default);

        Task<ProductReportDto> GetProductReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default);

        Task<CustomerReportDto> GetCustomerReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default);

        Task<ProviderReportDto> GetProviderReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default);
    }
}
