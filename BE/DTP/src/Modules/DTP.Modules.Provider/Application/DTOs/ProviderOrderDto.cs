using DTP.Modules.Provider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProviderOrderDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public Guid ProviderId { get; set; }

        public string ProviderName { get; set; } = default!;

        public string? ProviderOrderCode { get; set; }

        public ProviderOrderStatus Status { get; set; }

        public int RetryCount { get; set; }

        public string? ErrorCode { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
