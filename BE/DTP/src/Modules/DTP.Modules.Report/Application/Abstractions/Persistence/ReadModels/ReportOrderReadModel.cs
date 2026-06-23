using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Abstractions.Persistence.ReadModels
{
    public class ReportOrderReadModel
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;

        //public Guid? UserId { get; set; }
        public Guid? CustomerId { get; set; }

        public string? CustomerEmail { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }

        //public string CurrencyCode { get; set; } = "VND";
        public int Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        //public DateTime? CompletedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
