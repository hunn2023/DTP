using DTP.Modules.Notification.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.DTOs
{
    public class NotificationTemplateDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = default!;

        public NotificationType Type { get; set; }

        public NotificationChannel Channel { get; set; }

        public string TitleTemplate { get; set; } = default!;

        public string ContentTemplate { get; set; } = default!;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
