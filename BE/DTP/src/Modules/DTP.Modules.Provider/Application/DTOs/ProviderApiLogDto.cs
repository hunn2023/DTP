using DTP.Modules.Provider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProviderApiLogDto
    {
        public Guid Id { get; set; }

        public Guid ProviderId { get; set; }

        public string? ProviderName { get; set; }

        public ProviderApiLogType LogType { get; set; }

        public string Endpoint { get; set; } = default!;

        public string Method { get; set; } = default!;

        public string? RequestBody { get; set; }

        public string? ResponseBody { get; set; }

        public int? StatusCode { get; set; }

        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime RequestedAt { get; set; }

        public DateTime? RespondedAt { get; set; }

        public long? DurationMs { get; set; }
    }
}
