using DTP.Modules.Ordering.Domain.Enums;
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
            OrderItemType itemType,
            Guid productId,
            Guid? productVariantId,
            Guid? esimPackageId,
            Guid? phoneCardId,
            string productName,
            string? variantName,
            string? sku,
            int quantity,
            decimal unitPrice,
            string currency)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0.", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("Đơn giá không hợp lệ.", nameof(unitPrice));

            Id = Guid.NewGuid();
            OrderId = orderId;
            ItemType = itemType;
            ProductId = productId;
            ProductVariantId = productVariantId;
            EsimPackageId = esimPackageId;
            PhoneCardId = phoneCardId;
            ProductName = productName;
            VariantName = variantName;
            Sku = sku;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Currency = currency;
            TotalPrice = unitPrice * quantity;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid OrderId { get; private set; }

        public Order Order { get; private set; } = default!;

        public OrderItemType ItemType { get; private set; }

        public Guid ProductId { get; private set; }

        public Guid? ProductVariantId { get; private set; }

        public Guid? EsimPackageId { get; private set; }

        public Guid? PhoneCardId { get; private set; }

        public string ProductName { get; private set; } = default!;

        public string? VariantName { get; private set; }

        public string? Sku { get; private set; }

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal TotalPrice { get; private set; }

        public string Currency { get; private set; } = "VND";


        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Số lượng phải lớn hơn 0.");

            Quantity = quantity;
            TotalPrice = UnitPrice * quantity;
        }
    }
}
