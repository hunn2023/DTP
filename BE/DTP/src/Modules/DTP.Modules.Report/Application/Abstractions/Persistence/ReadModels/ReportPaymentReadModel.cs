using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Abstractions.Persistence.ReadModels
{
    public class ReportPaymentReadModel
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

        //public string PaymentCode { get; set; } = string.Empty;
        //public string PaymentMethod { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        //public string CurrencyCode { get; set; } = "VND";

        public int Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        //public DateTime? RefundedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
