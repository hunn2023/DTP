using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.DTOs
{
    public class CustomerReportDto
    {
        public int TotalCustomers { get; set; }
        public int NewCustomers { get; set; }
        public int ReturningCustomers { get; set; }

        public decimal TotalCustomerRevenue { get; set; }
        public decimal AverageRevenuePerCustomer { get; set; }

        public List<TimeSeriesPointDto> NewCustomersByDate { get; set; } = new();
        public List<TopItemDto> TopCustomers { get; set; } = new();
    }
}
