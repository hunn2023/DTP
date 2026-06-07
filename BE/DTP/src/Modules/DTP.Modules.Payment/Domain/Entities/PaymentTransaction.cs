using DTP.Modules.Payment.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Domain.Entities
{
    public class PaymentTransaction : EntityBase
    {
        private PaymentTransaction()
        {
        }

        public PaymentTransaction(
            Guid orderId,
            string orderCode,
            PaymentProviderCode providerCode,
            string transactionCode,
            decimal amount,
            string currencyCode)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            OrderCode = orderCode;
            ProviderCode = providerCode;
            TransactionCode = transactionCode;
            Amount = amount;
            CurrencyCode = currencyCode;
            Status = PaymentTransactionStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Guid OrderId { get; private set; }

        public string OrderCode { get; private set; } = default!;

        public PaymentProviderCode ProviderCode { get; private set; }

        public string TransactionCode { get; private set; } = default!;

        public string? ProviderTransactionCode { get; private set; }

        public decimal Amount { get; private set; }

        public string CurrencyCode { get; private set; } = "VND";

        public string? PaymentUrl { get; private set; }

        public string? QrCodeUrl { get; private set; }

        public string? QrContent { get; private set; }

        public PaymentTransactionStatus Status { get; private set; }

        public DateTime? ExpiredAt { get; private set; }

        public DateTime? PaidAt { get; private set; }

        public DateTime? FailedAt { get; private set; }

        public string? FailureReason { get; private set; }

        public string? RawRequest { get; private set; }

        public string? RawResponse { get; private set; }

        public void AttachProviderResult(
            string? providerTransactionCode,
            string? paymentUrl,
            string? qrCodeUrl,
            string? qrContent,
            DateTime? expiredAt,
            string? rawRequest,
            string? rawResponse)
        {
            ProviderTransactionCode = providerTransactionCode;
            PaymentUrl = paymentUrl;
            QrCodeUrl = qrCodeUrl;
            QrContent = qrContent;
            ExpiredAt = expiredAt;
            RawRequest = rawRequest;
            RawResponse = rawResponse;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkSuccess(string? providerTransactionCode = null)
        {
            if (Status == PaymentTransactionStatus.Success)
                return;

            ProviderTransactionCode = providerTransactionCode ?? ProviderTransactionCode;
            Status = PaymentTransactionStatus.Success;
            PaidAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string? reason)
        {
            if (Status == PaymentTransactionStatus.Success)
                throw new InvalidOperationException("Successful transaction cannot be marked as failed.");

            Status = PaymentTransactionStatus.Failed;
            FailureReason = reason;
            FailedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkExpired()
        {
            if (Status == PaymentTransactionStatus.Success)
                return;

            Status = PaymentTransactionStatus.Expired;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkCancelled()
        {
            if (Status == PaymentTransactionStatus.Success)
                throw new InvalidOperationException("Successful transaction cannot be cancelled.");

            Status = PaymentTransactionStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
