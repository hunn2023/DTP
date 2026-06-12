using DTP.Modules.Report.Application.Abstractions.Exports;
using DTP.Modules.Report.Application.Abstractions.Services;
using DTP.Modules.Report.Application.DTOs;
using DTP.Modules.Report.Domain.Enums;
using DTP.Shared.Application;
using System.Text;

namespace DTP.Modules.Report.Infrastructure.Exports
{
    public class CsvReportExportService : IReportExportService
    {
        private readonly IReportService _reportService;

        public CsvReportExportService(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<Result<ReportExportResultDto>> ExportAsync(
            ReportMetricType reportType,
            ReportExportFormat format,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            if (format != ReportExportFormat.Csv)
                return Result<ReportExportResultDto>.Failure("Unsupported export format.");

            return reportType switch
            {
                ReportMetricType.Sales => await ExportSalesAsync(fromDate, toDate, cancellationToken),
                ReportMetricType.Orders => await ExportOrdersAsync(fromDate, toDate, cancellationToken),
                ReportMetricType.Payments => await ExportPaymentsAsync(fromDate, toDate, cancellationToken),
                ReportMetricType.Products => await ExportProductsAsync(fromDate, toDate, cancellationToken),
                ReportMetricType.Customers => await ExportCustomersAsync(fromDate, toDate, cancellationToken),
                ReportMetricType.Providers => await ExportProvidersAsync(fromDate, toDate, cancellationToken),
                _ => Result<ReportExportResultDto>.Failure("This report type is not supported for CSV export.")
            };
        }

        private async Task<Result<ReportExportResultDto>> ExportSalesAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var report = await _reportService.GetSalesReportAsync(
                fromDate,
                toDate,
                ReportDateGroupType.Day,
                cancellationToken);

            if (!report.IsSuccess || report.Data == null)
                return Result<ReportExportResultDto>.Failure(report.Error ?? "Cannot get sales report.");

            var sb = new StringBuilder();

            sb.AppendLine("Date,Revenue,Count");

            foreach (var item in report.Data.RevenueByDate ?? [])
            {
                sb.AppendLine($"{Escape(item.Label)},{item.Value},{item.Count}");
            }

            return Result<ReportExportResultDto>.Success(
                CreateCsvResult("sales-report.csv", sb));
        }

        private async Task<Result<ReportExportResultDto>> ExportOrdersAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var report = await _reportService.GetOrderReportAsync(
                fromDate,
                toDate,
                ReportDateGroupType.Day,
                cancellationToken);

            if (!report.IsSuccess || report.Data == null)
                return Result<ReportExportResultDto>.Failure(report.Error ?? "Cannot get order report.");

            var data = report.Data;

            var sb = new StringBuilder();

            sb.AppendLine("Metric,Value");
            sb.AppendLine($"Total Orders,{data.TotalOrders}");
            sb.AppendLine($"Pending Orders,{data.PendingOrders}");
            sb.AppendLine($"Processing Orders,{data.ProcessingOrders}");
            sb.AppendLine($"Completed Orders,{data.CompletedOrders}");
            sb.AppendLine($"Cancelled Orders,{data.CancelledOrders}");
            sb.AppendLine($"Failed Orders,{data.FailedOrders}");
            sb.AppendLine($"Total Amount,{data.TotalOrderAmount}");
            sb.AppendLine($"Average Amount,{data.AverageOrderAmount}");

            return Result<ReportExportResultDto>.Success(
                CreateCsvResult("orders-report.csv", sb));
        }

        private async Task<Result<ReportExportResultDto>> ExportPaymentsAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var report = await _reportService.GetPaymentReportAsync(
                fromDate,
                toDate,
                ReportDateGroupType.Day,
                cancellationToken);

            if (!report.IsSuccess || report.Data == null)
                return Result<ReportExportResultDto>.Failure(report.Error ?? "Cannot get payment report.");

            var data = report.Data;

            var sb = new StringBuilder();

            sb.AppendLine("Metric,Value");
            sb.AppendLine($"Total Payments,{data.TotalPayments}");
            sb.AppendLine($"Success Payments,{data.SuccessPayments}");
            sb.AppendLine($"Pending Payments,{data.PendingPayments}");
            sb.AppendLine($"Failed Payments,{data.FailedPayments}");
            sb.AppendLine($"Refunded Payments,{data.RefundedPayments}");
            sb.AppendLine($"Total Paid Amount,{data.TotalPaidAmount}");
            sb.AppendLine($"Total Refunded Amount,{data.TotalRefundedAmount}");

            return Result<ReportExportResultDto>.Success(
                CreateCsvResult("payments-report.csv", sb));
        }

        private async Task<Result<ReportExportResultDto>> ExportProductsAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var report = await _reportService.GetProductReportAsync(
                fromDate,
                toDate,
                cancellationToken);

            if (!report.IsSuccess || report.Data == null)
                return Result<ReportExportResultDto>.Failure(report.Error ?? "Cannot get product report.");

            var sb = new StringBuilder();

            sb.AppendLine("Product,Code,Quantity,Revenue");

            foreach (var item in report.Data.TopSellingProducts ?? [])
            {
                sb.AppendLine($"{Escape(item.Name)},{Escape(item.Code)},{item.Count},{item.Value}");
            }

            return Result<ReportExportResultDto>.Success(
                CreateCsvResult("products-report.csv", sb));
        }

        private async Task<Result<ReportExportResultDto>> ExportCustomersAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var report = await _reportService.GetCustomerReportAsync(
                fromDate,
                toDate,
                ReportDateGroupType.Day,
                cancellationToken);

            if (!report.IsSuccess || report.Data == null)
                return Result<ReportExportResultDto>.Failure(report.Error ?? "Cannot get customer report.");

            var sb = new StringBuilder();

            sb.AppendLine("Customer,Code,Orders,Revenue");

            foreach (var item in report.Data.TopCustomers ?? [])
            {
                sb.AppendLine($"{Escape(item.Name)},{Escape(item.Code)},{item.Count},{item.Value}");
            }

            return Result<ReportExportResultDto>.Success(
                CreateCsvResult("customers-report.csv", sb));
        }

        private async Task<Result<ReportExportResultDto>> ExportProvidersAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var report = await _reportService.GetProviderReportAsync(
                fromDate,
                toDate,
                cancellationToken);

            if (!report.IsSuccess || report.Data == null)
                return Result<ReportExportResultDto>.Failure("Cannot get provider report.");

            var sb = new StringBuilder();

            sb.AppendLine("Provider,Code,Orders,Revenue");

            foreach (var item in report.Data.RevenueByProvider ?? [])
            {
                sb.AppendLine($"{Escape(item.Name)},{Escape(item.Code)},{item.Count},{item.Value}");
            }

            return Result<ReportExportResultDto>.Success(
                CreateCsvResult("providers-report.csv", sb));
        }

        private static ReportExportResultDto CreateCsvResult(
            string fileName,
            StringBuilder sb)
        {
            var content = Encoding.UTF8.GetPreamble()
                .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
                .ToArray();

            return new ReportExportResultDto
            {
                FileName = fileName,
                ContentType = "text/csv; charset=utf-8",
                Content = content
            };
        }

        private static string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}