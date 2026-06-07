using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.DTOs
{
    public class ProviderReportDto
    {
        public int TotalProviders { get; set; }
        public int ActiveProviders { get; set; }
        public int InactiveProviders { get; set; }

        public decimal TotalProviderRevenue { get; set; }
        public int TotalProviderOrders { get; set; }

        public List<TopItemDto> RevenueByProvider { get; set; } = new();
        public List<TopItemDto> OrdersByProvider { get; set; } = new();
    }
}
