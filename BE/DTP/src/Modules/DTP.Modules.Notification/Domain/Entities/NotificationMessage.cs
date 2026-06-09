using DTP.Modules.Notification.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Domain.Entities
{
    public class NotificationMessage : EntityBase
    {
        private NotificationMessage()
        {
        }

        public NotificationMessage(
            Guid? userId,
            string? email,
            NotificationType type,
            NotificationChannel channel,
            string title,
            string content,
            string? referenceType,
            Guid? referenceId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Email = email;
            Type = type;
            Channel = channel;
            Title = title;
            Content = content;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            Status = NotificationStatus.Pending;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid? UserId { get; private set; }

        public string? Email { get; private set; }

        public NotificationType Type { get; private set; }

        public NotificationChannel Channel { get; private set; }

        public NotificationStatus Status { get; private set; }

        public string Title { get; private set; } = default!;

        public string Content { get; private set; } = default!;

        public string? ReferenceType { get; private set; }

        public Guid? ReferenceId { get; private set; }

        public bool IsRead { get; private set; }

        public DateTime? ReadAt { get; private set; }

        public DateTime? SentAt { get; private set; }

        public string? ErrorMessage { get; private set; }

        public void MarkSent()
        {
            Status = NotificationStatus.Sent;
            SentAt = DateTime.UtcNow;
            ErrorMessage = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string errorMessage)
        {
            Status = NotificationStatus.Failed;
            ErrorMessage = errorMessage;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkRead()
        {
            IsRead = true;
            Status = NotificationStatus.Read;
            ReadAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
