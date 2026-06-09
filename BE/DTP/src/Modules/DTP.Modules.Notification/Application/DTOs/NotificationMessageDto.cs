using DTP.Modules.Notification.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.DTOs
{
    public class NotificationMessageDto
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public string? Email { get; set; }

        public NotificationType Type { get; set; }

        public NotificationChannel Channel { get; set; }

        public NotificationStatus Status { get; set; }

        public string Title { get; set; } = default!;

        public string Content { get; set; } = default!;

        public string? ReferenceType { get; set; }

        public Guid? ReferenceId { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime? SentAt { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
