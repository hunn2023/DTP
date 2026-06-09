using DTP.Modules.Delivery.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.DTOs
{
    public class DeliveryDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public Guid? CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerEmail { get; set; }

        public DeliveryType DeliveryType { get; set; }

        public DeliveryStatus Status { get; set; }

        public int AttemptCount { get; set; }

        public string? LastError { get; set; }

        public string? IpAddress { get; set; }

        public string? Note { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public DateTime? FailedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool EmailSent { get; set; }

        public DateTime? EmailSentAt { get; set; }

        public string? EmailError { get; set; }


        public List<DeliveryItemDto> Items { get; set; } = new();
    }
}
