using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.DTOs
{
    public class OrderDetailDto : OrderDto
    {
        public string? PaymentTransactionId { get; set; }

        public string? Note { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public string? CancelReason { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();

        public List<OrderHistoryDto> Histories { get; set; } = new();
    }
}
