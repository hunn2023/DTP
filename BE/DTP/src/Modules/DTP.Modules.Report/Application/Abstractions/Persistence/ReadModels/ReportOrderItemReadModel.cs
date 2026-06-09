using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Abstractions.Persistence.ReadModels
{
    public class ReportOrderItemReadModel
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public Guid? ProviderId { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public string? ProductCode { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
