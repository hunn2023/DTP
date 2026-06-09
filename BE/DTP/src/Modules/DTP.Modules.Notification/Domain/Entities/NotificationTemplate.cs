using DTP.Modules.Notification.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Domain.Entities
{
    public class NotificationTemplate : EntityBase
    {
        private NotificationTemplate()
        {
        }

        public NotificationTemplate(
            string code,
            NotificationType type,
            NotificationChannel channel,
            string titleTemplate,
            string contentTemplate,
            bool isActive)
        {
            Id = Guid.NewGuid();
            Code = code;
            Type = type;
            Channel = channel;
            TitleTemplate = titleTemplate;
            ContentTemplate = contentTemplate;
            IsActive = isActive;
            CreatedAt = DateTime.UtcNow;
        }

        public string Code { get; private set; } = default!;

        public NotificationType Type { get; private set; }

        public NotificationChannel Channel { get; private set; }

        public string TitleTemplate { get; private set; } = default!;

        public string ContentTemplate { get; private set; } = default!;

        public bool IsActive { get; private set; }

        public void Update(
            string titleTemplate,
            string contentTemplate,
            bool isActive)
        {
            TitleTemplate = titleTemplate;
            ContentTemplate = contentTemplate;
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Disable()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
