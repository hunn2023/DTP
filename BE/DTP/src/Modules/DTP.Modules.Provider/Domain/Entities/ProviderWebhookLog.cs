using DTP.Modules.Provider.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class ProviderWebhookLog : EntityBase
    {
        private ProviderWebhookLog()
        {
        }

        public ProviderWebhookLog(
            Guid providerId,
            string eventType,
            string payload)
        {
            Id = Guid.NewGuid();
            ProviderId = providerId;
            EventType = eventType.Trim();
            Payload = payload;
            Status = ProviderWebhookStatus.Received;
            ReceivedAt = DateTime.UtcNow;
        }

        public Guid ProviderId { get; private set; }

        public string EventType { get; private set; } = default!;

        public string Payload { get; private set; } = default!;

        public ProviderWebhookStatus Status { get; private set; }

        public string? ErrorMessage { get; private set; }

        public DateTime ReceivedAt { get; private set; }

        public DateTime? ProcessedAt { get; private set; }

        public void MarkProcessed()
        {
            Status = ProviderWebhookStatus.Processed;
            ProcessedAt = DateTime.UtcNow;
            ErrorMessage = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string errorMessage)
        {
            Status = ProviderWebhookStatus.Failed;
            ErrorMessage = errorMessage;
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
