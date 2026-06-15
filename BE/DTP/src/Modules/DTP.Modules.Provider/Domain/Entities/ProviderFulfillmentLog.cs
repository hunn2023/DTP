using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class ProviderFulfillmentLog : EntityBase
    {
        public Guid ProviderId { get; private set; }

        public Guid OrderId { get; private set; }
        public Guid OrderItemId { get; private set; }

        public string ProviderSku { get; private set; } = default!;

        public string Status { get; private set; } = default!;
        // Pending, Success, Failed

        public string? RequestBody { get; private set; }
        public string? ResponseBody { get; private set; }

        public string? QrCodeUrl { get; private set; }
        public string? ActivationCode { get; private set; }

        public string? ErrorMessage { get; private set; }

        private ProviderFulfillmentLog()
        {
        }

        public ProviderFulfillmentLog(
            Guid providerId,
            Guid orderId,
            Guid orderItemId,
            string providerSku,
            string? requestBody)
        {
            ProviderId = providerId;
            OrderId = orderId;
            OrderItemId = orderItemId;
            ProviderSku = providerSku;
            RequestBody = requestBody;
            Status = "Pending";
        }

        public void MarkSuccess(
            string? responseBody,
            string? qrCodeUrl,
            string? activationCode)
        {
            Status = "Success";
            ResponseBody = responseBody;
            QrCodeUrl = qrCodeUrl;
            ActivationCode = activationCode;
            ErrorMessage = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string? responseBody, string errorMessage)
        {
            Status = "Failed";
            ResponseBody = responseBody;
            ErrorMessage = errorMessage;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}