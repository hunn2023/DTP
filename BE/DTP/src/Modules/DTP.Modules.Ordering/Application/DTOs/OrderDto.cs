using DTP.Modules.Ordering.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }

        public string OrderCode { get; set; } = default!;

        public Guid? CustomerId { get; set; }

        public string? CustomerEmail { get; set; }

        public string? CustomerPhone { get; set; }

        public string? CustomerName { get; set; }

        public string Currency { get; set; } = "VND";

        public decimal SubTotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public OrderPaymentStatus PaymentStatus { get; set; }

        public string? PaymentMethod { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? PaidAt { get; set; }
    }

    public class MarkOrderPaidRequest
    {
        public string PaymentTransactionId { get; set; } = default!;

        public Guid? ChangedBy { get; set; }
    }

    public class CompleteOrderRequest
    {
        public Guid? ChangedBy { get; set; }
    }

    public class CancelOrderRequest
    {
        public string Reason { get; set; } = default!;

        public Guid? ChangedBy { get; set; }
    }

    public class ConfirmOrderRequest
    {
        public string? PaymentMethod { get; set; }

        public Guid? ChangedBy { get; set; }
    }
}
