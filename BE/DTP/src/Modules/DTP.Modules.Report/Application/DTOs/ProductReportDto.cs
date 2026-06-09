using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.DTOs
{
    public class ProductReportDto
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }

        public int TotalSoldQuantity { get; set; }
        public decimal TotalProductRevenue { get; set; }

        public List<TopItemDto> TopSellingProducts { get; set; } = new();
        public List<TopItemDto> LowSellingProducts { get; set; } = new();
        public List<TopItemDto> RevenueByCategory { get; set; } = new();
    }
}
