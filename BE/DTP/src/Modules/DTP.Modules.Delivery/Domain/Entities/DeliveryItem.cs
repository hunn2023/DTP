using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Domain.Entities
{
    public class DeliveryItem : EntityBase
    {
        private DeliveryItem()
        {
        }

        public DeliveryItem(
            Guid deliveryId,
            Guid? orderItemId,
            Guid productId,
            Guid? productVariantId,
            string productName,
            string? sku,
            int quantity)
        {
            Id = Guid.NewGuid();
            DeliveryId = deliveryId;
            OrderItemId = orderItemId;
            ProductId = productId;
            ProductVariantId = productVariantId;
            ProductName = productName;
            Sku = sku;
            Quantity = quantity;
            IsDelivered = false;
        }

        public Guid DeliveryId { get; private set; }

        public Delivery Delivery { get; private set; } = default!;

        public Guid? OrderItemId { get; private set; }

        public Guid ProductId { get; private set; }

        public Guid? ProductVariantId { get; private set; }

        public string ProductName { get; private set; } = default!;

        public string? Sku { get; private set; }

        public int Quantity { get; private set; }

        public string? QrCodeUrl { get; private set; }

        public string? ActivationCode { get; private set; }

        public string? SerialNumber { get; private set; }

        public string? ProviderReference { get; private set; }

        public string? RawData { get; private set; }

        public bool IsDelivered { get; private set; }

        public DateTime? DeliveredAt { get; private set; }

        public void SetFulfillment(
            string? qrCodeUrl,
            string? activationCode,
            string? serialNumber,
            string? providerReference,
            string? rawData)
        {
            QrCodeUrl = qrCodeUrl;
            ActivationCode = activationCode;
            SerialNumber = serialNumber;
            ProviderReference = providerReference;
            RawData = rawData;
        }

        public void MarkDelivered()
        {
            IsDelivered = true;
            DeliveredAt = DateTime.UtcNow;
        }
    }
}
