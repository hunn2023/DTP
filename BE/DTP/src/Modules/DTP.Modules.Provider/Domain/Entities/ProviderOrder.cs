using DTP.Modules.Provider.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class ProviderOrder : EntityBase
    {
        private readonly List<ProviderOrderItem> _items = new();

        private ProviderOrder()
        {
        }

        public ProviderOrder(
            Guid orderId,
            string orderCode,
            Guid providerId)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            OrderCode = orderCode.Trim();
            ProviderId = providerId;
            Status = ProviderOrderStatus.Pending;
            RetryCount = 0;
        }

        public Guid OrderId { get; private set; }

        public string OrderCode { get; private set; } = default!;

        public Guid ProviderId { get; private set; }

        public ExternalProvider Provider { get; private set; } = default!;

        public string? ProviderOrderCode { get; private set; }

        public ProviderOrderStatus Status { get; private set; }

        public int RetryCount { get; private set; }

        public string? ErrorCode { get; private set; }

        public string? ErrorMessage { get; private set; }

        public DateTime? SentAt { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        public IReadOnlyCollection<ProviderOrderItem> Items => _items.AsReadOnly();

        public void AddItem(
            Guid orderItemId,
            Guid productId,
            Guid productVariantId,
            string providerProductCode,
            int quantity)
        {
            var item = new ProviderOrderItem(
                Id,
                orderItemId,
                productId,
                productVariantId,
                providerProductCode,
                quantity);

            _items.Add(item);
        }

        public void MarkProcessing()
        {
            Status = ProviderOrderStatus.Processing;
            SentAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkSuccess(string providerOrderCode)
        {
            ProviderOrderCode = providerOrderCode;
            Status = ProviderOrderStatus.Success;
            CompletedAt = DateTime.UtcNow;
            ErrorCode = null;
            ErrorMessage = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkWaitingWebhook(string providerOrderCode)
        {
            ProviderOrderCode = providerOrderCode;
            Status = ProviderOrderStatus.WaitingWebhook;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string? errorCode, string errorMessage)
        {
            Status = ProviderOrderStatus.Failed;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            RetryCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            Status = ProviderOrderStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
