using DTP.Modules.Notification.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Domain.Entities
{
    public class NotificationDeliveryLog : EntityBase
    {
        private NotificationDeliveryLog()
        {
        }

        public NotificationDeliveryLog(
            Guid notificationMessageId,
            NotificationChannel channel,
            NotificationStatus status,
            string? provider,
            string? requestPayload,
            string? responsePayload,
            string? errorMessage)
        {
            Id = Guid.NewGuid();
            NotificationMessageId = notificationMessageId;
            Channel = channel;
            Status = status;
            Provider = provider;
            RequestPayload = requestPayload;
            ResponsePayload = responsePayload;
            ErrorMessage = errorMessage;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid NotificationMessageId { get; private set; }

        public NotificationChannel Channel { get; private set; }

        public NotificationStatus Status { get; private set; }

        public string? Provider { get; private set; }

        public string? RequestPayload { get; private set; }

        public string? ResponsePayload { get; private set; }

        public string? ErrorMessage { get; private set; }
    }
}
