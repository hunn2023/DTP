using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.DTOs
{
    public class SalesReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalRefund { get; set; }
        public decimal NetRevenue { get; set; }

        public int TotalOrders { get; set; }
        public int PaidOrders { get; set; }
        public int CancelledOrders { get; set; }

        public decimal AverageOrderValue { get; set; }

        public List<TimeSeriesPointDto> RevenueByDate { get; set; } = new();
        public List<TopItemDto> RevenueByProduct { get; set; } = new();
        public List<TopItemDto> RevenueByProvider { get; set; } = new();
    }
}
