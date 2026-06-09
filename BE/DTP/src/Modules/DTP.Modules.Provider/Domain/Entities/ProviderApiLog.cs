using DTP.Modules.Provider.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class ProviderApiLog : EntityBase
    {
        private ProviderApiLog()
        {
        }

        public ProviderApiLog(
            Guid providerId,
            ProviderApiLogType logType,
            string endpoint,
            string method,
            string? requestBody)
        {
            Id = Guid.NewGuid();
            ProviderId = providerId;
            LogType = logType;
            Endpoint = endpoint.Trim();
            Method = method.Trim().ToUpperInvariant();
            RequestBody = requestBody;
            RequestedAt = DateTime.UtcNow;
            IsSuccess = false;
        }

        public Guid ProviderId { get; private set; }

        public ExternalProvider Provider { get; private set; } = default!;

        public ProviderApiLogType LogType { get; private set; }

        public string Endpoint { get; private set; } = default!;

        public string Method { get; private set; } = default!;

        public string? RequestBody { get; private set; }

        public string? ResponseBody { get; private set; }

        public int? StatusCode { get; private set; }

        public bool IsSuccess { get; private set; }

        public string? ErrorMessage { get; private set; }

        public DateTime RequestedAt { get; private set; }

        public DateTime? RespondedAt { get; private set; }

        public long? DurationMs { get; private set; }

        public void SetResponse(
            int? statusCode,
            string? responseBody,
            bool isSuccess,
            string? errorMessage,
            long? durationMs)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            DurationMs = durationMs;
            RespondedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
