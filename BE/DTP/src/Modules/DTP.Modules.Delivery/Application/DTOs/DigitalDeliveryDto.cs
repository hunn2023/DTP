using DTP.Modules.Delivery.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.DTOs
{
    public class DigitalDeliveryDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid OrderItemId { get; set; }

        public Guid EsimProfileId { get; set; }

        public string RecipientEmail { get; set; } = default!;

        public DigitalDeliveryStatus Status { get; set; }

        public string StatusName { get; set; } = default!;

        public DateTime? DeliveredAt { get; set; }

        public string? FailedReason { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
