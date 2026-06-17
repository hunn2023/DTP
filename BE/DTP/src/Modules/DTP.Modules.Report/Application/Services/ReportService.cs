using DTP.Modules.Report.Application.Abstractions.Repositories;
using DTP.Modules.Report.Application.Abstractions.Services;
using DTP.Modules.Report.Application.DTOs;
using DTP.Modules.Report.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Caching;

namespace DTP.Modules.Report.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly ICacheService _cacheService;

        public ReportService(
            IReportRepository reportRepository,
            ICacheService cacheService)
        {
            _reportRepository = reportRepository;
            _cacheService = cacheService;
        }

        public async Task<Result<DashboardReportDto>> GetDashboardReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            var normalizedFromDate = NormalizeFromDate(fromDate);
            var normalizedToDate = NormalizeToDate(toDate);

            if (normalizedFromDate > normalizedToDate)
                return Result<DashboardReportDto>.Failure("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

            var cacheKey = BuildDashboardReportCacheKey(
                normalizedFromDate,
                normalizedToDate);

            var cachedResult = await _cacheService.GetAsync<DashboardReportDto>(
                cacheKey,
                cancellationToken);

            if (cachedResult != null)
                return Result<DashboardReportDto>.Success(cachedResult);

            var report = await _reportRepository.GetDashboardReportAsync(
                normalizedFromDate,
                normalizedToDate,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                report,
                TimeSpan.FromMinutes(3),
                cancellationToken);

            return Result<DashboardReportDto>.Success(report);
        }

        public async Task<Result<SalesReportDto>> GetSalesReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default)
        {
            var normalizedFromDate = NormalizeFromDate(fromDate);
            var normalizedToDate = NormalizeToDate(toDate);

            if (normalizedFromDate > normalizedToDate)
                return Result<SalesReportDto>.Failure("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

            var cacheKey = BuildSalesReportCacheKey(
                normalizedFromDate,
                normalizedToDate,
                groupType);

            var cachedResult = await _cacheService.GetAsync<SalesReportDto>(
                cacheKey,
                cancellationToken);

            if (cachedResult != null)
                return Result<SalesReportDto>.Success(cachedResult);

            var report = await _reportRepository.GetSalesReportAsync(
                normalizedFromDate,
                normalizedToDate,
                groupType,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                report,
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return Result<SalesReportDto>.Success(report);
        }

        public async Task<Result<OrderReportDto>> GetOrderReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default)
        {
            var normalizedFromDate = NormalizeFromDate(fromDate);
            var normalizedToDate = NormalizeToDate(toDate);

            if (normalizedFromDate > normalizedToDate)
                return Result<OrderReportDto>.Failure("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

            var cacheKey = BuildOrderReportCacheKey(
                normalizedFromDate,
                normalizedToDate,
                groupType);

            var cachedResult = await _cacheService.GetAsync<OrderReportDto>(
                cacheKey,
                cancellationToken);

            if (cachedResult != null)
                return Result<OrderReportDto>.Success(cachedResult);

            var report = await _reportRepository.GetOrderReportAsync(
                normalizedFromDate,
                normalizedToDate,
                groupType,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                report,
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return Result<OrderReportDto>.Success(report);
        }

        public async Task<Result<PaymentReportDto>> GetPaymentReportAsync(
               DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default)
        {
            var normalizedFromDate = NormalizeFromDate(fromDate);
            var normalizedToDate = NormalizeToDate(toDate);

            if (normalizedFromDate > normalizedToDate)
                return Result<PaymentReportDto>.Failure("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

            var cacheKey = BuildPaymentReportCacheKey(
                normalizedFromDate,
                normalizedToDate,
                groupType);

            var cachedResult = await _cacheService.GetAsync<PaymentReportDto>(
                cacheKey,
                cancellationToken);

            if (cachedResult != null)
                return Result<PaymentReportDto>.Success(cachedResult);

            var report = await _reportRepository.GetPaymentReportAsync(
                normalizedFromDate,
                normalizedToDate,
                groupType,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                report,
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return Result<PaymentReportDto>.Success(report);
        }

        public async Task<Result<ProductReportDto>> GetProductReportAsync(
                DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            var normalizedFromDate = NormalizeFromDate(fromDate);
            var normalizedToDate = NormalizeToDate(toDate);

            if (normalizedFromDate > normalizedToDate)
                return Result<ProductReportDto>.Failure("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

            var cacheKey = BuildProductReportCacheKey(
                normalizedFromDate,
                normalizedToDate);

            var cachedResult = await _cacheService.GetAsync<ProductReportDto>(
                cacheKey,
                cancellationToken);

            if (cachedResult != null)
                return Result<ProductReportDto>.Success(cachedResult);

            var report = await _reportRepository.GetProductReportAsync(
                normalizedFromDate,
                normalizedToDate,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                report,
                TimeSpan.FromMinutes(10),
                cancellationToken);

            return Result<ProductReportDto>.Success(report);
        }

        public async Task<Result<CustomerReportDto>> GetCustomerReportAsync(
                DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default)
        {
            var normalizedFromDate = NormalizeFromDate(fromDate);
            var normalizedToDate = NormalizeToDate(toDate);

            if (normalizedFromDate > normalizedToDate)
                return Result<CustomerReportDto>.Failure("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

            var cacheKey = BuildCustomerReportCacheKey(
                normalizedFromDate,
                normalizedToDate,
                groupType);

            var cachedResult = await _cacheService.GetAsync<CustomerReportDto>(
                cacheKey,
                cancellationToken);

            if (cachedResult != null)
                return Result<CustomerReportDto>.Success(cachedResult);

            var report = await _reportRepository.GetCustomerReportAsync(
                normalizedFromDate,
                normalizedToDate,
                groupType,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                report,
                TimeSpan.FromMinutes(10),
                cancellationToken);

            return Result<CustomerReportDto>.Success(report);
        }

        public async Task<Result<ProviderReportDto>> GetProviderReportAsync(
              DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            var normalizedFromDate = NormalizeFromDate(fromDate);
            var normalizedToDate = NormalizeToDate(toDate);

            if (normalizedFromDate > normalizedToDate)
                return Result<ProviderReportDto>.Failure("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

            var cacheKey = BuildProviderReportCacheKey(
                normalizedFromDate,
                normalizedToDate);

            var cachedResult = await _cacheService.GetAsync<ProviderReportDto>(
                cacheKey,
                cancellationToken);

            if (cachedResult != null)
                return Result<ProviderReportDto>.Success(cachedResult);

            var report = await _reportRepository.GetProviderReportAsync(
                normalizedFromDate,
                normalizedToDate,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                report,
                TimeSpan.FromMinutes(10),
                cancellationToken);

            return Result<ProviderReportDto>.Success(report);
        }

        private static DateTime NormalizeFromDate(DateTime? fromDate)
        {
            var sourceDate = fromDate ?? DateTime.UtcNow;

            var firstDayOfMonth = new DateTime(
                sourceDate.Year,
                sourceDate.Month,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc);

            return firstDayOfMonth;
        }

        private static DateTime NormalizeToDate(DateTime? toDate)
        {
            var sourceDate = toDate ?? DateTime.UtcNow;

            var firstDayOfMonth = new DateTime(
                sourceDate.Year,
                sourceDate.Month,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc);

            var lastDayOfMonth = firstDayOfMonth
                .AddMonths(1)
                .AddTicks(-1);

            return lastDayOfMonth;
        }

        private static string BuildDashboardReportCacheKey(
               DateTime? fromDate,
            DateTime? toDate)
        {
            return $"admin:report:dashboard:{fromDate:yyyyMMdd}:{toDate:yyyyMMdd}";
        }

        private static string BuildSalesReportCacheKey(
              DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType)
        {
            return $"admin:report:sales:{fromDate:yyyyMMdd}:{toDate:yyyyMMdd}:{groupType}";
        }

        private static string BuildOrderReportCacheKey(
              DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType)
        {
            return $"admin:report:orders:{fromDate:yyyyMMdd}:{toDate:yyyyMMdd}:{groupType}";
        }

        private static string BuildPaymentReportCacheKey(
               DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType)
        {
            return $"admin:report:payments:{fromDate:yyyyMMdd}:{toDate:yyyyMMdd}:{groupType}";
        }

        private static string BuildProductReportCacheKey(
               DateTime? fromDate,
            DateTime? toDate)
        {
            return $"admin:report:products:{fromDate:yyyyMMdd}:{toDate:yyyyMMdd}";
        }

        private static string BuildCustomerReportCacheKey(
               DateTime? fromDate,
            DateTime? toDate,
            ReportDateGroupType groupType)
        {
            return $"admin:report:customers:{fromDate:yyyyMMdd}:{toDate:yyyyMMdd}:{groupType}";
        }

        private static string BuildProviderReportCacheKey(
              DateTime? fromDate,
            DateTime? toDate)
        {
            return $"admin:report:providers:{fromDate:yyyyMMdd}:{toDate:yyyyMMdd}";
        }


    }
}
