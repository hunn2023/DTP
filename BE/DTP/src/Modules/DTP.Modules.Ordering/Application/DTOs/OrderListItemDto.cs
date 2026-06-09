using DTP.Modules.Ordering.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.DTOs
{
    public class OrderListItemDto
    {
        public Guid Id { get; set; }

        public string OrderCode { get; set; } = default!;

        public Guid UserId { get; set; }

        public string CustomerEmail { get; set; } = default!;
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        public decimal TotalAmount { get; set; }
        public string CurrencyCode { get; set; } = "VND";

        public OrderStatus Status { get; set; }
        public string StatusName { get; set; } = default!;

        public OrderPaymentStatus PaymentStatus { get; set; }
        public string PaymentStatusName { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }

        public string? PaymentTransactionCode { get; set; }

        public string? IpAddress { get; set; }
        public string? CheckoutSource { get; set; }
    }
}
