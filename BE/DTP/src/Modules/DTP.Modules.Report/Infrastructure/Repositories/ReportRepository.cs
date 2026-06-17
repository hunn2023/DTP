using DTP.Modules.Report.Application.Abstractions.Persistence;
using DTP.Modules.Report.Application.Abstractions.Repositories;
using DTP.Modules.Report.Application.Common;
using DTP.Modules.Report.Application.DTOs;
using DTP.Modules.Report.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Report.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IReportReadDbContext _context;

        public ReportRepository(IReportReadDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardReportDto> GetDashboardReportAsync(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var orders = _context.OrderQuery
                .Where(x => !x.IsDeleted);

            var ordersInRange = orders
                .Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate);

            var paidPayments = _context.PaymentQuery
                .Where(x => !x.IsDeleted && x.Status == 2);

            var paidPaymentsInRange = paidPayments
                .Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate);

            var dto = new DashboardReportDto
            {
                TotalRevenue = await paidPayments.SumAsync(x => x.Amount, cancellationToken),
                TodayRevenue = await paidPayments
                    .Where(x => x.CreatedAt >= today)
                    .SumAsync(x => x.Amount, cancellationToken),
                MonthRevenue = await paidPayments
                    .Where(x => x.CreatedAt >= monthStart)
                    .SumAsync(x => x.Amount, cancellationToken),

                TotalOrders = await orders.CountAsync(cancellationToken),
                TodayOrders = await orders
                    .Where(x => x.CreatedAt >= today)
                    .CountAsync(cancellationToken),
                MonthOrders = await orders
                    .Where(x => x.CreatedAt >= monthStart)
                    .CountAsync(cancellationToken),

                CompletedOrders = await orders.CountAsync(x => x.Status == 4, cancellationToken),
                PendingOrders = await orders.CountAsync(x => x.Status == 1, cancellationToken),
                CancelledOrders = await orders.CountAsync(x => x.Status == 5, cancellationToken),

                TotalCustomers = await _context.CustomerQuery
                    .Where(x => !x.IsDeleted)
                    .CountAsync(cancellationToken),
                NewCustomersToday = await _context.CustomerQuery
                    .Where(x => !x.IsDeleted && x.CreatedAt >= today)
                    .CountAsync(cancellationToken),
                NewCustomersThisMonth = await _context.CustomerQuery
                    .Where(x => !x.IsDeleted && x.CreatedAt >= monthStart)
                    .CountAsync(cancellationToken),

                TotalPaidAmount = await paidPayments.SumAsync(x => x.Amount, cancellationToken),
                TotalRefundAmount = await _context.PaymentQuery
                    .Where(x => !x.IsDeleted && x.Status == 4)
                    .SumAsync(x => x.Amount, cancellationToken)
            };

            dto.RevenueChart = await BuildPaymentTimeSeriesAsync(
                fromDate,
                toDate,
                ReportDateGroupType.Day,
                cancellationToken);

            dto.OrderChart = await BuildOrderTimeSeriesAsync(
                fromDate,
                toDate,
                ReportDateGroupType.Day,
                cancellationToken);

            dto.TopProducts = await GetTopProductsAsync(
                fromDate,
                toDate,
                5,
                cancellationToken);

            dto.TopProviders = await GetTopProvidersAsync(
                fromDate,
                toDate,
                5,
                cancellationToken);

            return dto;
        }

        public async Task<SalesReportDto> GetSalesReportAsync(
            DateTime fromDate,
            DateTime toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default)
        {
            var paidPayments = _context.PaymentQuery
                .Where(x =>
                    !x.IsDeleted &&
                    x.Status == 2 &&
                    x.CreatedAt >= fromDate &&
                    x.CreatedAt <= toDate);

            var orders = _context.OrderQuery
                .Where(x =>
                    !x.IsDeleted &&
                    x.CreatedAt >= fromDate &&
                    x.CreatedAt <= toDate);

            var totalRevenue = await paidPayments.SumAsync(x => x.Amount, cancellationToken);
            var totalOrders = await orders.CountAsync(cancellationToken);
            var paidOrders = await orders.CountAsync(x => x.Status == 4, cancellationToken);
            var cancelledOrders = await orders.CountAsync(x => x.Status == 5, cancellationToken);
            var totalDiscount = await orders.SumAsync(x => x.DiscountAmount, cancellationToken);

            var totalRefund = await _context.PaymentQuery
                .Where(x =>
                    !x.IsDeleted &&
                    x.Status == 4 &&
                    x.CreatedAt >= fromDate &&
                    x.CreatedAt <= toDate)
                .SumAsync(x => x.Amount, cancellationToken);

            return new SalesReportDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalRevenue = totalRevenue,
                TotalDiscount = totalDiscount,
                TotalRefund = totalRefund,
                NetRevenue = totalRevenue - totalRefund,
                TotalOrders = totalOrders,
                PaidOrders = paidOrders,
                CancelledOrders = cancelledOrders,
                AverageOrderValue = totalOrders == 0 ? 0 : totalRevenue / totalOrders,
                RevenueByDate = await BuildPaymentTimeSeriesAsync(
                    fromDate,
                    toDate,
                    groupType,
                    cancellationToken),
                RevenueByProduct = await GetTopProductsAsync(
                    fromDate,
                    toDate,
                    20,
                    cancellationToken),
                RevenueByProvider = await GetTopProvidersAsync(
                    fromDate,
                    toDate,
                    20,
                    cancellationToken)
            };
        }

        public async Task<OrderReportDto> GetOrderReportAsync(
            DateTime fromDate,
            DateTime toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default)
        {
            var orders = _context.OrderQuery
                .Where(x =>
                    !x.IsDeleted &&
                    x.CreatedAt >= fromDate &&
                    x.CreatedAt <= toDate);

            var totalOrders = await orders.CountAsync(cancellationToken);
            var totalAmount = await orders.SumAsync(x => x.TotalAmount, cancellationToken);

            return new OrderReportDto
            {
                TotalOrders = totalOrders,
                PendingOrders = await orders.CountAsync(x => x.Status == 1, cancellationToken),
                ProcessingOrders = await orders.CountAsync(x => x.Status == 2, cancellationToken),
                CompletedOrders = await orders.CountAsync(x => x.Status == 4, cancellationToken),
                CancelledOrders = await orders.CountAsync(x => x.Status == 5, cancellationToken),
                FailedOrders = await orders.CountAsync(x => x.Status == 6, cancellationToken),
                TotalOrderAmount = totalAmount,
                AverageOrderAmount = totalOrders == 0 ? 0 : totalAmount / totalOrders,
                OrdersByDate = await BuildOrderTimeSeriesAsync(
                    fromDate,
                    toDate,
                    groupType,
                    cancellationToken),
                OrdersByStatus = await orders
                    .GroupBy(x => x.Status)
                    .Select(g => new TopItemDto
                    {
                        Code = g.Key.ToString(),
                        Name = GetOrderStatusName(g.Key),
                        Count = g.Count(),
                        Value = g.Sum(x => x.TotalAmount)
                    })
                    .ToListAsync(cancellationToken)
            };
        }

        public async Task<PaymentReportDto> GetPaymentReportAsync(
            DateTime fromDate,
            DateTime toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken = default)
        {
            var payments = _context.PaymentQuery
                .Where(x =>
                    !x.IsDeleted &&
                    x.CreatedAt >= fromDate &&
                    x.CreatedAt <= toDate);

            return new PaymentReportDto
            {
                TotalPayments = await payments.CountAsync(cancellationToken),
                SuccessPayments = await payments.CountAsync(x => x.Status == 2, cancellationToken),
                PendingPayments = await payments.CountAsync(x => x.Status == 1, cancellationToken),
                FailedPayments = await payments.CountAsync(x => x.Status == 3, cancellationToken),
                RefundedPayments = await payments.CountAsync(x => x.Status == 4, cancellationToken),

                TotalPaidAmount = await payments
                    .Where(x => x.Status == 2)
                    .SumAsync(x => x.Amount, cancellationToken),
                TotalRefundedAmount = await payments
                    .Where(x => x.Status == 4)
                    .SumAsync(x => x.Amount, cancellationToken),

                PaymentsByDate = await BuildPaymentTimeSeriesAsync(
                    fromDate,
                    toDate,
                    groupType,
                    cancellationToken),

                //PaymentsByMethod = await payments
                //    .GroupBy(x => x.PaymentMethod)
                //    .Select(g => new TopItemDto
                //    {
                //        Code = g.Key,
                //        Name = g.Key,
                //        Count = g.Count(),
                //        Value = g.Sum(x => x.Amount)
                //    })
                //    .OrderByDescending(x => x.Value)
                //    .ToListAsync(cancellationToken)

            };
        }

        public async Task<ProductReportDto> GetProductReportAsync(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken = default)
        {
            var products = _context.ProductQuery.Where(x => !x.IsDeleted);

            var orderItemsInRange =
                from oi in _context.OrderItemQuery
                join o in _context.OrderQuery on oi.OrderId equals o.Id
                where !oi.IsDeleted
                      && !o.IsDeleted
                      && o.CreatedAt >= fromDate
                      && o.CreatedAt <= toDate
                select oi;

            var topProducts = await GetTopProductsAsync(
                fromDate,
                toDate,
                20,
                cancellationToken);

            var lowSellingProducts = await (
                from p in products
                join oi in orderItemsInRange on p.Id equals oi.ProductId into gj
                from x in gj.DefaultIfEmpty()
                group x by new
                {
                    p.Id,
                    p.Code,
                    p.Name
                }
                into g
                select new TopItemDto
                {
                    Id = g.Key.Id,
                    Code = g.Key.Code ?? string.Empty,
                    Name = g.Key.Name,
                    Count = g.Sum(x => x == null ? 0 : x.Quantity),
                    Value = g.Sum(x => x == null ? 0 : x.TotalPrice)
                })
                .OrderBy(x => x.Count)
                .Take(20)
                .ToListAsync(cancellationToken);

            var revenueByCategory = await (
                from oi in orderItemsInRange
                join p in _context.ProductQuery on oi.ProductId equals p.Id
                join c in _context.CategoryQuery on p.CategoryId equals c.Id
                group oi by new
                {
                    c.Id,
                    c.Code,
                    c.Name
                }
                into g
                select new TopItemDto
                {
                    Id = g.Key.Id,
                    Code = g.Key.Code ?? string.Empty,
                    Name = g.Key.Name,
                    Count = g.Sum(x => x.Quantity),
                    Value = g.Sum(x => x.TotalPrice)
                })
                .OrderByDescending(x => x.Value)
                .ToListAsync(cancellationToken);

            return new ProductReportDto
            {
                TotalProducts = await products.CountAsync(cancellationToken),
                ActiveProducts = await products.CountAsync(x => x.IsActive, cancellationToken),
                InactiveProducts = await products.CountAsync(x => !x.IsActive, cancellationToken),
                TotalSoldQuantity = await orderItemsInRange.SumAsync(x => x.Quantity, cancellationToken),
                TotalProductRevenue = await orderItemsInRange.SumAsync(x => x.TotalPrice, cancellationToken),
                TopSellingProducts = topProducts,
                LowSellingProducts = lowSellingProducts,
                RevenueByCategory = revenueByCategory
            };
        }

        public async Task<CustomerReportDto> GetCustomerReportAsync(
                DateTime fromDate,
                DateTime toDate,
                ReportDateGroupType groupType,
                CancellationToken cancellationToken = default)
        {
            var customers =
                from u in _context.CustomerQuery
                join ur in _context.UserRoleQuery on u.Id equals ur.UserId
                join r in _context.RoleQuery on ur.RoleId equals r.Id
                where !u.IsDeleted
                      && r.Name == "Customer"
                select u;

            var newCustomers = customers
                .Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate);

            var ordersInRange = _context.OrderQuery
                .Where(x =>
                    !x.IsDeleted &&
                    x.CustomerId != null &&
                    x.CreatedAt >= fromDate &&
                    x.CreatedAt <= toDate);

            var customerRevenue = await ordersInRange
                .GroupBy(o => new
                {
                    o.CustomerId,
                    o.CustomerEmail,
                    o.CustomerName,
                    o.CustomerPhone
                })
                .Select(g => new TopItemDto
                {
                    Id = g.Key.CustomerId,
                    Code = g.Key.CustomerPhone ?? string.Empty,
                    Name = g.Key.CustomerName ?? g.Key.CustomerEmail ?? "Unknown",
                    Count = g.Count(),
                    Value = g.Sum(x => x.TotalAmount)
                })
                .OrderByDescending(x => x.Value)
                .Take(20)
                .ToListAsync(cancellationToken);

            var totalRevenue = await ordersInRange
                .SumAsync(x => x.TotalAmount, cancellationToken);

            var totalCustomers = await customers
                .CountAsync(cancellationToken);

            var newCustomerCount = await newCustomers
                .CountAsync(cancellationToken);

            var returningCustomerCount = await ordersInRange
                .GroupBy(x => x.CustomerId)
                .Where(g => g.Count() > 1)
                .CountAsync(cancellationToken);

            return new CustomerReportDto
            {
                TotalCustomers = totalCustomers,
                NewCustomers = newCustomerCount,
                ReturningCustomers = returningCustomerCount,
                TotalCustomerRevenue = totalRevenue,
                AverageRevenuePerCustomer = totalCustomers == 0
                    ? 0
                    : totalRevenue / totalCustomers,
                NewCustomersByDate = await BuildCustomerTimeSeriesAsync(
                    fromDate,
                    toDate,
                    groupType,
                    cancellationToken),
                TopCustomers = customerRevenue
            };
        }
        public async Task<ProviderReportDto> GetProviderReportAsync(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken = default)
        {
            var providers = _context.ProviderQuery.Where(x => !x.IsDeleted);

            var revenueByProvider = await GetTopProvidersAsync(
                fromDate,
                toDate,
                30,
                cancellationToken);

            return new ProviderReportDto
            {
                TotalProviders = await providers.CountAsync(cancellationToken),
                ActiveProviders = await providers.CountAsync(x => x.IsActive, cancellationToken),
                InactiveProviders = await providers.CountAsync(x => !x.IsActive, cancellationToken),
                TotalProviderRevenue = revenueByProvider.Sum(x => x.Value),
                TotalProviderOrders = revenueByProvider.Sum(x => x.Count),
                RevenueByProvider = revenueByProvider,
                OrdersByProvider = revenueByProvider
                    .OrderByDescending(x => x.Count)
                    .ToList()
            };
        }

        private async Task<List<TimeSeriesPointDto>> BuildPaymentTimeSeriesAsync(
            DateTime fromDate,
            DateTime toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken)
        {
            var data = await _context.PaymentQuery
                .Where(x =>
                    !x.IsDeleted &&
                    x.Status == 2 &&
                    x.CreatedAt >= fromDate &&
                    x.CreatedAt <= toDate)
                .Select(x => new
                {
                    x.CreatedAt,
                    x.Amount
                })
                .ToListAsync(cancellationToken);

            return data
                .GroupBy(x => ReportPeriodHelper.GetGroupDate(x.CreatedAt, groupType))
                .Select(g => new TimeSeriesPointDto
                {
                    Date = g.Key,
                    Label = ReportPeriodHelper.GetLabel(g.Key, groupType),
                    Value = g.Sum(x => x.Amount),
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();
        }

        private async Task<List<TimeSeriesPointDto>> BuildOrderTimeSeriesAsync(
            DateTime fromDate,
            DateTime toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken)
        {
            var data = await _context.OrderQuery
                .Where(x =>
                    !x.IsDeleted &&
                    x.CreatedAt >= fromDate &&
                    x.CreatedAt <= toDate)
                .Select(x => new
                {
                    x.CreatedAt,
                    x.TotalAmount
                })
                .ToListAsync(cancellationToken);

            return data
                .GroupBy(x => ReportPeriodHelper.GetGroupDate(x.CreatedAt, groupType))
                .Select(g => new TimeSeriesPointDto
                {
                    Date = g.Key,
                    Label = ReportPeriodHelper.GetLabel(g.Key, groupType),
                    Value = g.Sum(x => x.TotalAmount),
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();
        }

        private async Task<List<TimeSeriesPointDto>> BuildCustomerTimeSeriesAsync(
            DateTime fromDate,
            DateTime toDate,
            ReportDateGroupType groupType,
            CancellationToken cancellationToken)
        {
            var data = await _context.CustomerQuery
                .Where(x =>
                    !x.IsDeleted &&
                    x.CreatedAt >= fromDate &&
                    x.CreatedAt <= toDate)
                .Select(x => new
                {
                    x.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return data
                .GroupBy(x => ReportPeriodHelper.GetGroupDate(x.CreatedAt, groupType))
                .Select(g => new TimeSeriesPointDto
                {
                    Date = g.Key,
                    Label = ReportPeriodHelper.GetLabel(g.Key, groupType),
                    Count = g.Count(),
                    Value = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();
        }

        private async Task<List<TopItemDto>> GetTopProductsAsync(
             DateTime fromDate,
             DateTime toDate,
             int take,
             CancellationToken cancellationToken)
        {
            return await (
                from oi in _context.OrderItemQuery
                join o in _context.OrderQuery on oi.OrderId equals o.Id
                join p in _context.ProductQuery on oi.ProductId equals p.Id
                where !oi.IsDeleted
                      && !o.IsDeleted
                      && o.CreatedAt >= fromDate
                      && o.CreatedAt <= toDate
                group new { oi, p } by new
                {
                    oi.ProductId,
                    ProductCode = p.Code,
                    ProductName = p.Name
                }
                into g
                select new TopItemDto
                {
                    Id = g.Key.ProductId,
                    Code = g.Key.ProductCode ?? string.Empty,
                    Name = g.Key.ProductName ?? "Unknown",
                    Count = g.Sum(x => x.oi.Quantity),
                    Value = g.Sum(x => x.oi.TotalPrice)
                })
                .OrderByDescending(x => x.Value)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        private async Task<List<TopItemDto>> GetTopProvidersAsync(
                DateTime fromDate,
                DateTime toDate,
                int take,
                CancellationToken cancellationToken)
        {
            return await (
                from oi in _context.OrderItemQuery
                join o in _context.OrderQuery on oi.OrderId equals o.Id
                join ep in _context.EsimPackageQuery on oi.EsimPackageId equals ep.Id
                join p in _context.ProviderQuery on ep.ProviderId equals p.Id
                where !oi.IsDeleted
                      && !o.IsDeleted
                      && !p.IsDeleted
                      && ep.IsActive
                      && o.CreatedAt >= fromDate
                      && o.CreatedAt <= toDate
                group oi by new
                {
                    p.Id,
                    p.Code,
                    p.Name
                }
                into g
                select new TopItemDto
                {
                    Id = g.Key.Id,
                    Code = g.Key.Code ?? string.Empty,
                    Name = g.Key.Name ?? "Unknown",
                    Count = g.Sum(x => x.Quantity),
                    Value = g.Sum(x => x.TotalPrice)
                })
                .OrderByDescending(x => x.Value)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        private static string GetOrderStatusName(int status)
        {
            return status switch
            {
                1 => "Pending",
                2 => "Processing",
                3 => "Paid",
                4 => "Completed",
                5 => "Cancelled",
                6 => "Failed",
                _ => "Unknown"
            };
        }
    }
}
