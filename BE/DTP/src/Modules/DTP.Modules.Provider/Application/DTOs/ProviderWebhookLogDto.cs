using DTP.Modules.Provider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProviderWebhookLogDto
    {
        public Guid Id { get; set; }

        public Guid ProviderId { get; set; }

        public string? ProviderName { get; set; }

        public string EventType { get; set; } = default!;

        public string Payload { get; set; } = default!;

        public ProviderWebhookStatus Status { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime ReceivedAt { get; set; }

        public DateTime? ProcessedAt { get; set; }
    }
}
