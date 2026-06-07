using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.DTOs
{
    public class PaymentReportDto
    {
        public int TotalPayments { get; set; }
        public int SuccessPayments { get; set; }
        public int PendingPayments { get; set; }
        public int FailedPayments { get; set; }
        public int RefundedPayments { get; set; }

        public decimal TotalPaidAmount { get; set; }
        public decimal TotalRefundedAmount { get; set; }

        public List<TimeSeriesPointDto> PaymentsByDate { get; set; } = new();
        public List<TopItemDto> PaymentsByMethod { get; set; } = new();
    }
}
