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
           string endpoint,
           string method,
           string? requestBody,
           string? responseBody,
           int? statusCode,
           bool isSuccess,
           string? errorMessage)
        {
            ProviderId = providerId;
            Endpoint = endpoint;
            Method = method;
            RequestBody = requestBody;
            ResponseBody = responseBody;
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }


        public Guid ProviderId { get; private set; }

        public string Endpoint { get; private set; } = default!;

        public string Method { get; private set; } = default!;

        public string? RequestBody { get; private set; }

        public string? ResponseBody { get; private set; }

        public int? StatusCode { get; private set; }

        public bool IsSuccess { get; private set; }

        public string? ErrorMessage { get; private set; }

    }
}
