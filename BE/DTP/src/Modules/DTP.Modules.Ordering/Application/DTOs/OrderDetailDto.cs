using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.DTOs
{
    public class OrderDetailDto
    {
        public Guid Id { get; set; }

        public string OrderCode { get; set; } = default!;

        public Guid UserId { get; set; }

        public string CustomerEmail { get; set; } = default!;
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        public string CurrencyCode { get; set; } = "VND";

        public decimal SubtotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public int Status { get; set; }
        public string StatusName { get; set; } = default!;

        public int PaymentStatus { get; set; }
        public string PaymentStatusName { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? ReferrerUrl { get; set; }
        public string? CheckoutSource { get; set; }


        public List<OrderItemDto> Items { get; set; } = new();
    }
}
