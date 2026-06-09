using DTP.Modules.Notification.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.DTOs
{
    public class NotificationDeliveryLogDto
    {
        public Guid Id { get; set; }

        public Guid NotificationMessageId { get; set; }

        public NotificationChannel Channel { get; set; }

        public NotificationStatus Status { get; set; }

        public string? Provider { get; set; }

        public string? RequestPayload { get; set; }

        public string? ResponsePayload { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
