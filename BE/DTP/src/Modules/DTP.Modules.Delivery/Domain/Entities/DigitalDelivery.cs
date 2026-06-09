using DTP.Modules.Delivery.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Domain.Entities
{
    public class DigitalDelivery : EntityBase
    {
        private DigitalDelivery()
        {
        }

        public DigitalDelivery(
            Guid orderId,
            Guid orderItemId,
            Guid esimProfileId,
            string recipientEmail)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            OrderItemId = orderItemId;
            EsimProfileId = esimProfileId;
            RecipientEmail = recipientEmail;
            Status = DigitalDeliveryStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Guid OrderId { get; private set; }

        public Guid OrderItemId { get; private set; }

        public Guid EsimProfileId { get; private set; }

        public string RecipientEmail { get; private set; } = default!;

        public DigitalDeliveryStatus Status { get; private set; }

        public DateTime? DeliveredAt { get; private set; }

        public string? FailedReason { get; private set; }

        public void MarkDelivered()
        {
            Status = DigitalDeliveryStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
            FailedReason = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string reason)
        {
            Status = DigitalDeliveryStatus.Failed;
            FailedReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkRetrying()
        {
            Status = DigitalDeliveryStatus.Retrying;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
