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
            Guid? customerId,
            decimal amount,
            string currency,
            Guid paymentProviderId,
            string paymentProviderCode,
            PaymentMethod method,
            string requestId,
            string ipAddress)
        {

            OrderId = orderId;
            OrderCode = orderCode;
            CustomerId = customerId;
            Amount = amount;
            Currency = currency;
            PaymentProviderId = paymentProviderId;
            PaymentProviderCode = paymentProviderCode;
            Method = method;
            RequestId = requestId;
            Status = PaymentStatus.Pending;
            IpAddress = ipAddress;

        }

        public Guid OrderId { get; private set; }

        public string OrderCode { get; private set; } = default!;

        public Guid? CustomerId { get; private set; }

        public decimal Amount { get; private set; }

        public string Currency { get; private set; } = "VND";


        public Guid PaymentProviderId { get; private set; }
        public string PaymentProviderCode { get; private set; } = default!;


        public PaymentMethod Method { get; private set; }

        public PaymentStatus Status { get; private set; }

        public string RequestId { get; private set; } = default!;

        public string? ProviderTransactionId { get; private set; }

        public string? ProviderPaymentCode { get; private set; }

        public string? QrCode { get; private set; }

        public string? QrImageUrl { get; private set; }

        public string? PaymentUrl { get; private set; }

        public DateTime? ExpiredAt { get; private set; }

        public DateTime? PaidAt { get; private set; }

        public string? BankCode { get; private set; }

        public string? BankAccountNo { get; private set; }

        public string? BankAccountName { get; private set; }

        public string? TransferContent { get; private set; }

        public string? ProviderResponseCode { get; private set; }

        public string? ProviderResponseMessage { get; private set; }

        public string? RawProviderRequest { get; private set; }

        public string? RawProviderResponse { get; private set; }

        public string? RawCallbackData { get; private set; }

        public string? IpAddress { get; private set; }

        public void MarkQrCreated(
            string? providerTransactionId,
            string? providerPaymentCode,
            string? qrCode,
            string? qrImageUrl,
            string? paymentUrl,
            DateTime? expiredAt,
            string? bankCode,
            string? bankAccountNo,
            string? bankAccountName,
            string? transferContent,
            string? providerResponseCode,
            string? providerResponseMessage,
            string? rawProviderRequest,
            string? rawProviderResponse)
        {
            ProviderTransactionId = providerTransactionId;
            ProviderPaymentCode = providerPaymentCode;
            QrCode = qrCode;
            QrImageUrl = qrImageUrl;
            PaymentUrl = paymentUrl;
            ExpiredAt = expiredAt;
            BankCode = bankCode;
            BankAccountNo = bankAccountNo;
            BankAccountName = bankAccountName;
            TransferContent = transferContent;
            ProviderResponseCode = providerResponseCode;
            ProviderResponseMessage = providerResponseMessage;
            RawProviderRequest = rawProviderRequest;
            RawProviderResponse = rawProviderResponse;
            Status = PaymentStatus.Pending;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkProcessing(string? rawCallbackData)
        {
            if (Status == PaymentStatus.Paid)
                return;

            RawCallbackData = rawCallbackData;
            Status = PaymentStatus.Processing;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkPaid(
            string? providerTransactionId,
            string? providerResponseCode,
            string? providerResponseMessage,
            string? rawCallbackData)
        {
            if (Status == PaymentStatus.Paid)
                return;

            ProviderTransactionId = string.IsNullOrWhiteSpace(providerTransactionId)
                ? ProviderTransactionId
                : providerTransactionId;

            ProviderResponseCode = providerResponseCode;
            ProviderResponseMessage = providerResponseMessage;
            RawCallbackData = rawCallbackData;
            Status = PaymentStatus.Paid;
            PaidAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(
            string? providerResponseCode,
            string? providerResponseMessage,
            string? rawCallbackData)
        {
            if (Status == PaymentStatus.Paid)
                return;

            ProviderResponseCode = providerResponseCode;
            ProviderResponseMessage = providerResponseMessage;
            RawCallbackData = rawCallbackData;
            Status = PaymentStatus.Failed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkExpired()
        {
            if (Status == PaymentStatus.Paid)
                return;

            Status = PaymentStatus.Expired;
            UpdatedAt = DateTime.UtcNow;
        }


        public void MarkCreateQrFailed(
            string? providerResponseCode,
            string? providerResponseMessage,
            string? rawProviderResponse)
        {
            if (Status == PaymentStatus.Paid)
                return;

            ProviderResponseCode = providerResponseCode;
            ProviderResponseMessage = providerResponseMessage;
            RawProviderResponse = rawProviderResponse;
            Status = PaymentStatus.Failed;
            UpdatedAt = DateTime.UtcNow;
        }


        public bool IsPaid()
        {
            return Status == PaymentStatus.Paid;
        }
    }
}
