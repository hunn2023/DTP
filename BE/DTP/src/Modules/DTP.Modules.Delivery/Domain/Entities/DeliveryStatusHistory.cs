using DTP.Modules.Delivery.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Domain.Entities
{
    public class DeliveryStatusHistory : EntityBase
    {
        private DeliveryStatusHistory()
        {
        }

        public DeliveryStatusHistory(
            Guid deliveryId,
            DeliveryStatus status,
            string message,
            string? detail)
        {
            Id = Guid.NewGuid();
            DeliveryId = deliveryId;
            Status = status;
            Message = message;
            Detail = detail;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid DeliveryId { get; private set; }

        public Delivery Delivery { get; private set; } = default!;

        public DeliveryStatus Status { get; private set; }

        public string Message { get; private set; } = default!;

        public string? Detail { get; private set; }

    }
}
