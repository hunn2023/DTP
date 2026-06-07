using DTP.Modules.Report.Application.Abstractions.Exports;
using DTP.Modules.Report.Application.Abstractions.Services;
using DTP.Modules.Report.Application.DTOs;
using DTP.Modules.Report.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Infrastructure.Exports
{
    public class CsvReportExportService : IReportExportService
    {
        private readonly IReportService _reportService;

        public CsvReportExportService(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<ReportExportResultDto> ExportAsync(
            ReportMetricType reportType,
            ReportExportFormat format,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            if (format != ReportExportFormat.Csv)
                throw new NotSupportedException("Currently only CSV export is supported.");

            return reportType switch
            {
                ReportMetricType.Sales => await ExportSalesAsync(fromDate, toDate, cancellationToken),
                ReportMetricType.Orders => await ExportOrdersAsync(fromDate, toDate, cancellationToken),
                ReportMetricType.Payments => await ExportPaymentsAsync(fromDate, toDate, cancellationToken),
                ReportMetricType.Products => await ExportProductsAsync(fromDate, toDate, cancellationToken),
                ReportMetricType.Customers => await ExportCustomersAsync(fromDate, toDate, cancellationToken),
                ReportMetricType.Providers => await ExportProvidersAsync(fromDate, toDate, cancellationToken),
                _ => throw new NotSupportedException("This report type is not supported for export.")
            };
        }

        private async Task<ReportExportResultDto> ExportSalesAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var report = await _reportService.GetSalesReportAsync(
                fromDate,
                toDate,
                ReportDateGroupType.Day,
                cancellationToken);

            var sb = new StringBuilder();

            sb.AppendLine("Date,Revenue,Count");

            foreach (var item in report.RevenueByDate)
            {
                sb.AppendLine($"{Escape(item.Label)},{item.Value},{item.Count}");
            }

            return CreateCsvResult("sales-report.csv", sb);
        }

        private async Task<ReportExportResultDto> ExportOrdersAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var report = await _reportService.GetOrderReportAsync(
                fromDate,
                toDate,
                ReportDateGroupType.Day,
                cancellationToken);

            var sb = new StringBuilder();

            sb.AppendLine("Metric,Value");
            sb.AppendLine($"Total Orders,{report.TotalOrders}");
            sb.AppendLine($"Pending Orders,{report.PendingOrders}");
            sb.AppendLine($"Processing Orders,{report.ProcessingOrders}");
            sb.AppendLine($"Completed Orders,{report.CompletedOrders}");
            sb.AppendLine($"Cancelled Orders,{report.CancelledOrders}");
            sb.AppendLine($"Failed Orders,{report.FailedOrders}");
            sb.AppendLine($"Total Amount,{report.TotalOrderAmount}");
            sb.AppendLine($"Average Amount,{report.AverageOrderAmount}");

            return CreateCsvResult("order-report.csv", sb);
        }

        private async Task<ReportExportResultDto> ExportPaymentsAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var report = await _reportService.GetPaymentReportAsync(
                fromDate,
                toDate,
                ReportDateGroupType.Day,
                cancellationToken);

            var sb = new StringBuilder();

            sb.AppendLine("Metric,Value");
            sb.AppendLine($"Total Payments,{report.TotalPayments}");
            sb.AppendLine($"Success Payments,{report.SuccessPayments}");
            sb.AppendLine($"Pending Payments,{report.PendingPayments}");
            sb.AppendLine($"Failed Payments,{report.FailedPayments}");
            sb.AppendLine($"Refunded Payments,{report.RefundedPayments}");
            sb.AppendLine($"Total Paid Amount,{report.TotalPaidAmount}");
            sb.AppendLine($"Total Refunded Amount,{report.TotalRefundedAmount}");

            return CreateCsvResult("payment-report.csv", sb);
        }

        private async Task<ReportExportResultDto> ExportProductsAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var report = await _reportService.GetProductReportAsync(
                fromDate,
                toDate,
                cancellationToken);

            var sb = new StringBuilder();

            sb.AppendLine("Product,Code,Quantity,Revenue");

            foreach (var item in report.TopSellingProducts)
            {
                sb.AppendLine($"{Escape(item.Name)},{Escape(item.Code)},{item.Count},{item.Value}");
            }

            return CreateCsvResult("product-report.csv", sb);
        }

        private async Task<ReportExportResultDto> ExportCustomersAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var report = await _reportService.GetCustomerReportAsync(
                fromDate,
                toDate,
                ReportDateGroupType.Day,
                cancellationToken);

            var sb = new StringBuilder();

            sb.AppendLine("Customer,Code,Orders,Revenue");

            foreach (var item in report.TopCustomers)
            {
                sb.AppendLine($"{Escape(item.Name)},{Escape(item.Code)},{item.Count},{item.Value}");
            }

            return CreateCsvResult("customer-report.csv", sb);
        }

        private async Task<ReportExportResultDto> ExportProvidersAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var report = await _reportService.GetProviderReportAsync(
                fromDate,
                toDate,
                cancellationToken);

            var sb = new StringBuilder();

            sb.AppendLine("Provider,Code,Orders,Revenue");

            foreach (var item in report.RevenueByProvider)
            {
                sb.AppendLine($"{Escape(item.Name)},{Escape(item.Code)},{item.Count},{item.Value}");
            }

            return CreateCsvResult("provider-report.csv", sb);
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
                ContentType = "text/csv",
                Content = content
            };
        }

        private static string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}