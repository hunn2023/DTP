using DTP.Modules.Delivery.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Domain.Entities
{
    public class DeliveryLog : EntityBase
    {
        private DeliveryLog()
        {
        }

        public DeliveryLog(
            Guid orderId,
            Guid? orderItemId,
            string action,
            DeliveryLogStatus status,
            string? message,
            string? rawData = null)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            OrderItemId = orderItemId;
            Action = action;
            Status = status;
            Message = message;
            RawData = rawData;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid OrderId { get; private set; }

        public Guid? OrderItemId { get; private set; }

        public string Action { get; private set; } = default!;

        public DeliveryLogStatus Status { get; private set; }

        public string? Message { get; private set; }

        public string? RawData { get; private set; }
    }
}
