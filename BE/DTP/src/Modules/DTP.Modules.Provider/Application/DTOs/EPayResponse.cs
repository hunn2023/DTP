using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public sealed class EPayResponse
    {
        [JsonPropertyName("responseCode")]
        public string ResponseCode { get; set; } = string.Empty;

        [JsonPropertyName("responseMessage")]
        public string ResponseMessage { get; set; } = string.Empty;
    }
}
