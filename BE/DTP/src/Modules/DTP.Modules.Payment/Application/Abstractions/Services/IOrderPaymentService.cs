using DTP.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface IOrderPaymentService
    {
        Task<OrderPaymentInfo?> GetOrderPaymentInfoAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task<Result> MarkOrderPaidAsync(
            Guid orderId,
            Guid paymentTransactionId,
            string? providerTransactionId,
            DateTime paidAt,
            CancellationToken cancellationToken = default);
    }

    public class OrderPaymentInfo
    {
        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public Guid? CustomerId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; } = "VND";

        public string Status { get; set; } = default!;

        public DateTime? PaymentExpiredAt { get; set; }
    }
}
