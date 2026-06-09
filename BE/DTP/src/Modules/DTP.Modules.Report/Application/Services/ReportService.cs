using DTP.Modules.Report.Application.Abstractions.Repositories;
using DTP.Modules.Report.Application.Abstractions.Services;
using DTP.Modules.Report.Application.Common;
using DTP.Modules.Report.Application.DTOs;
using DTP.Modules.Report.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;

        public ReportService(IReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<DashboardReportDto> GetDashboardReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            var range = new ReportDateRange(fromDate, toDate);

            return await _repository.GetDashboardReportAsync(
                range.FromDate,
                range.ToDate,
                cancellationToken);
        }

        public async Task<SalesReportDto> GetSalesReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default)
        {
            var range = new ReportDateRange(fromDate, toDate);

            return await _repository.GetSalesReportAsync(
                range.FromDate,
                range.ToDate,
                groupType,
                cancellationToken);
        }

        public async Task<OrderReportDto> GetOrderReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default)
        {
            var range = new ReportDateRange(fromDate, toDate);

            return await _repository.GetOrderReportAsync(
                range.FromDate,
                range.ToDate,
                groupType,
                cancellationToken);
        }

        public async Task<PaymentReportDto> GetPaymentReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default)
        {
            var range = new ReportDateRange(fromDate, toDate);

            return await _repository.GetPaymentReportAsync(
                range.FromDate,
                range.ToDate,
                groupType,
                cancellationToken);
        }

        public async Task<ProductReportDto> GetProductReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            var range = new ReportDateRange(fromDate, toDate);

            return await _repository.GetProductReportAsync(
                range.FromDate,
                range.ToDate,
                cancellationToken);
        }

        public async Task<CustomerReportDto> GetCustomerReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default)
        {
            var range = new ReportDateRange(fromDate, toDate);

            return await _repository.GetCustomerReportAsync(
                range.FromDate,
                range.ToDate,
                groupType,
                cancellationToken);
        }

        public async Task<ProviderReportDto> GetProviderReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            var range = new ReportDateRange(fromDate, toDate);

            return await _repository.GetProviderReportAsync(
                range.FromDate,
                range.ToDate,
                cancellationToken);
        }
    }
}
