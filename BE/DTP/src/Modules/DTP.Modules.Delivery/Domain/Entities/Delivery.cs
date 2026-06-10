using DTP.Modules.Delivery.Domain.Enums;
using DTP.Shared.Domain;

namespace DTP.Modules.Delivery.Domain.Entities
{
    public class Delivery : EntityBase
    {
        private readonly List<DeliveryItem> _items = new();
        private readonly List<DeliveryStatusHistory> _histories = new();

        private Delivery()
        {
        }

        public Delivery(
            Guid orderId,
            string orderCode,
            Guid? customerId,
            string? customerName,
            string? customerEmail,
            DeliveryType deliveryType,
            string? ipAddress)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            OrderCode = orderCode;
            CustomerId = customerId;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            DeliveryType = deliveryType;
            Status = DeliveryStatus.Pending;
            AttemptCount = 0;
            IpAddress = ipAddress;
            CreatedAt = DateTime.UtcNow;

            AddHistory(
                DeliveryStatus.Pending,
                "Delivery created.",
                null);
        }

        public Guid OrderId { get; private set; }

        public string OrderCode { get; private set; } = default!;

        public Guid? CustomerId { get; private set; }

        public string? CustomerName { get; private set; }

        public string? CustomerEmail { get; private set; }

        public DeliveryType DeliveryType { get; private set; }

        public DeliveryStatus Status { get; private set; }

        public int AttemptCount { get; private set; }

        public string? LastError { get; private set; }

        public string? IpAddress { get; private set; }

        public string? Note { get; private set; }

        public DateTime? DeliveredAt { get; private set; }

        public DateTime? FailedAt { get; private set; }

        public bool EmailSent { get; private set; }

        public DateTime? EmailSentAt { get; private set; }

        public string? EmailError { get; private set; }

        public IReadOnlyCollection<DeliveryItem> Items => _items;

        public IReadOnlyCollection<DeliveryStatusHistory> Histories => _histories;

        public void AddItem(
            Guid? orderItemId,
            Guid productId,
            Guid? productVariantId,
            string productName,
            string? sku,
            int quantity)
        {
            if (quantity <= 0)
                quantity = 1;

            var item = new DeliveryItem(
                Id,
                orderItemId,
                productId,
                productVariantId,
                productName,
                sku,
                quantity);

            _items.Add(item);
        }

        public void StartProcessing()
        {
            if (Status == DeliveryStatus.Delivered)
                return;

            Status = DeliveryStatus.Processing;
            AttemptCount += 1;
            LastError = null;

            AddHistory(
                DeliveryStatus.Processing,
                "Delivery processing started.",
                null);
        }

        public void MarkDelivered(string? note = null)
        {
            Status = DeliveryStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
            FailedAt = null;
            LastError = null;
            Note = note;

            foreach (var item in _items)
            {
                item.MarkDelivered();
            }

            AddHistory(
                DeliveryStatus.Delivered,
                "Delivery completed successfully.",
                note);
        }

        public void MarkFailed(string error)
        {
            Status = DeliveryStatus.Failed;
            LastError = error;
            FailedAt = DateTime.UtcNow;

            AddHistory(
                DeliveryStatus.Failed,
                "Delivery failed.",
                error);
        }

        public void Cancel(string? reason)
        {
            if (Status == DeliveryStatus.Delivered)
                return;

            Status = DeliveryStatus.Cancelled;
            Note = reason;

            AddHistory(
                DeliveryStatus.Cancelled,
                "Delivery cancelled.",
                reason);
        }

        public void SetItemFulfillment(
            Guid deliveryItemId,
            string? qrCodeUrl,
            string? activationCode,
            string? serialNumber,
            string? providerReference,
            string? rawData)
        {
            var item = _items.FirstOrDefault(x => x.Id == deliveryItemId);

            if (item == null)
                return;

            item.SetFulfillment(
                qrCodeUrl,
                activationCode,
                serialNumber,
                providerReference,
                rawData);
        }

        private void AddHistory(
            DeliveryStatus status,
            string message,
            string? detail)
        {
            _histories.Add(new DeliveryStatusHistory(
                Id,
                status,
                message,
                detail));
        }

        public void MarkEmailSent()
        {
            EmailSent = true;
            EmailSentAt = DateTime.UtcNow;
            EmailError = null;
        }

        public void MarkEmailFailed(string error)
        {
            EmailSent = false;
            EmailError = error;
        }



    }
}
