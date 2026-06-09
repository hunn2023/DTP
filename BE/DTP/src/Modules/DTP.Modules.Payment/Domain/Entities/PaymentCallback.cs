using DTP.Modules.Payment.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Domain.Entities
{
    public class PaymentCallback : EntityBase
    {
        private PaymentCallback()
        {
        }

        public PaymentCallback(
            Guid? paymentTransactionId,
            PaymentProvider providerCode,
            string? orderCode,
            string? transactionCode,
            string? providerTransactionCode,
            string requestBody,
            string? signature,
            bool isValidSignature)
        {
            Id = Guid.NewGuid();
            PaymentTransactionId = paymentTransactionId;
            ProviderCode = providerCode;
            OrderCode = orderCode;
            TransactionCode = transactionCode;
            ProviderTransactionCode = providerTransactionCode;
            RequestBody = requestBody;
            Signature = signature;
            IsValidSignature = isValidSignature;
            Status = isValidSignature
                ? PaymentCallbackStatus.Received
                : PaymentCallbackStatus.InvalidSignature;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid? PaymentTransactionId { get; private set; }

        public PaymentProvider ProviderCode { get; private set; }

        public string? OrderCode { get; private set; }

        public string? TransactionCode { get; private set; }

        public string? ProviderTransactionCode { get; private set; }

        public string RequestBody { get; private set; } = default!;

        public string? Signature { get; private set; }

        public bool IsValidSignature { get; private set; }

        public PaymentCallbackStatus Status { get; private set; }

        public DateTime? ProcessedAt { get; private set; }

        public string? ErrorMessage { get; private set; }

        public void MarkProcessed()
        {
            Status = PaymentCallbackStatus.Processed;
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string errorMessage)
        {
            Status = PaymentCallbackStatus.Failed;
            ErrorMessage = errorMessage;
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkIgnored(string reason)
        {
            Status = PaymentCallbackStatus.Ignored;
            ErrorMessage = reason;
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
