using DTP.Modules.Notification.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.DTOs
{
    public class CreateNotificationRequest
    {
        public Guid? UserId { get; set; }

        public string? Email { get; set; }

        public NotificationType Type { get; set; }

        public NotificationChannel Channel { get; set; }

        public string? TemplateCode { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? ReferenceType { get; set; }

        public Guid? ReferenceId { get; set; }

        public Dictionary<string, string>? Data { get; set; }
    }
}
