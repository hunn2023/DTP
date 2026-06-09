using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.DTOs
{
    public class OrderReportDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int FailedOrders { get; set; }

        public decimal TotalOrderAmount { get; set; }
        public decimal AverageOrderAmount { get; set; }

        public List<TimeSeriesPointDto> OrdersByDate { get; set; } = new();
        public List<TopItemDto> OrdersByStatus { get; set; } = new();
    }
}
