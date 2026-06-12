using DTP.Modules.Report.Application.DTOs;
using DTP.Modules.Report.Domain.Enums;
using DTP.Shared.Application;


namespace DTP.Modules.Report.Application.Abstractions.Services
{
    public interface IReportService
    {
        Task<Result<DashboardReportDto>> GetDashboardReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default);

        Task<Result<SalesReportDto>> GetSalesReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default);

        Task<Result<OrderReportDto>> GetOrderReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default);

        Task<Result<PaymentReportDto>> GetPaymentReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default);

        Task<Result<ProductReportDto>> GetProductReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default);

        Task<Result<CustomerReportDto>> GetCustomerReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default);

        Task<Result<ProviderReportDto>> GetProviderReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default);
    }
}
