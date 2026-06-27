using DTP.Modules.Payment.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Domain.Entities
{
    public class PaymentCallbackLog : EntityBase
    {
        private PaymentCallbackLog()
        {
        }

        public PaymentCallbackLog(
            Guid paymentProviderId,
            string? requestId,
            string? providerTransactionId,
            string rawBody,
            string? signature,
            string? ipAddress,
            PaymentCallbackStatus status)
        {
            Id = Guid.NewGuid();
            PaymentProviderId = paymentProviderId;
            RequestId = requestId;
            ProviderTransactionId = providerTransactionId;
            RawBody = rawBody;
            Signature = signature;
            IpAddress = ipAddress;
            Status = status;
            ReceivedAt = DateTime.UtcNow;
        }

        public Guid PaymentProviderId { get; private set; }

        public Guid? PaymentTransactionId { get; private set; }

        public string? RequestId { get; private set; }

        public string? ProviderTransactionId { get; private set; }

        public string RawBody { get; private set; } = default!;

        public string? Signature { get; private set; }

        public string? IpAddress { get; private set; }

        public PaymentCallbackStatus Status { get; private set; }

        public string? ErrorMessage { get; private set; }

        public DateTime ReceivedAt { get; private set; }

        public DateTime? ProcessedAt { get; private set; }

        public void AttachPayment(Guid paymentTransactionId)
        {
            PaymentTransactionId = paymentTransactionId;
        }

        public void MarkVerified()
        {
            Status = PaymentCallbackStatus.Verified;
        }

        public void MarkProcessed()
        {
            Status = PaymentCallbackStatus.Processed;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkInvalidSignature(string? errorMessage)
        {
            Status = PaymentCallbackStatus.InvalidSignature;
            ErrorMessage = errorMessage;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkDuplicated(string? errorMessage)
        {
            Status = PaymentCallbackStatus.Duplicated;
            ErrorMessage = errorMessage;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string? errorMessage)
        {
            Status = PaymentCallbackStatus.Failed;
            ErrorMessage = errorMessage;
            ProcessedAt = DateTime.UtcNow;
        }
    }
}
