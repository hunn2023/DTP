using DTP.Shared.Domain;

namespace DTP.Modules.Ordering.Domain.Entities
{
    public class OrderItem : EntityBase
    {
        private OrderItem()
        {
        }

        public OrderItem(
            Guid orderId,
            Guid productId,
            Guid? productVariantId,
            Guid? esimPackageId,
            Guid? phoneCardId,
            string productCode,
            string productName,
            string? productSlug,
            string? variantName,
            string? sku,
            string? thumbnailUrl,
            int quantity,
            decimal unitPrice,
            string currencyCode)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            ProductVariantId = productVariantId;
            EsimPackageId = esimPackageId;
            PhoneCardId = phoneCardId;

            ProductCode = productCode;
            ProductName = productName;
            ProductSlug = productSlug;
            VariantName = variantName;
            Sku = sku;
            ThumbnailUrl = thumbnailUrl;

            Quantity = quantity;
            UnitPrice = unitPrice;
            CurrencyCode = currencyCode;
            TotalPrice = unitPrice * quantity;
        }

        public Guid OrderId { get; private set; }

        public Guid ProductId { get; private set; }
        public Guid? ProductVariantId { get; private set; }

        public Guid? EsimPackageId { get; private set; }
        public Guid? PhoneCardId { get; private set; }

        public string ProductCode { get; private set; } = default!;
        public string ProductName { get; private set; } = default!;
        public string? ProductSlug { get; private set; }
        public string? VariantName { get; private set; }
        public string? Sku { get; private set; }
        public string? ThumbnailUrl { get; private set; }

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice { get; private set; }
        public string CurrencyCode { get; private set; } = "VND";

        public Order Order { get; private set; } = default!;
    }
}
