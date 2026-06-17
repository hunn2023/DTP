using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.DTOs
{
    public class DashboardReportDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal MonthRevenue { get; set; }

        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }
        public int MonthOrders { get; set; }

        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }

        public int TotalCustomers { get; set; }
        public int NewCustomersToday { get; set; }
        public int NewCustomersThisMonth { get; set; }

        public decimal TotalPaidAmount { get; set; }
        public decimal TotalRefundAmount { get; set; }

        public List<TimeSeriesPointDto> RevenueChart { get; set; } = new();
        public List<TimeSeriesPointDto> OrderChart { get; set; } = new();
        public List<TopItemDto> TopProducts { get; set; } = new();
        public List<TopItemDto> TopProviders { get; set; } = new();

        public List<TopItemDto> TopCountries { get; set; } = new();
        public List<TopItemDto> TopRegions { get; set; } = new();
    }
}
