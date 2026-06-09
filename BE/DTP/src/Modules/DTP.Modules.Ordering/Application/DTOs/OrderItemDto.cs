using DTP.Modules.Ordering.Domain.Enums;


namespace DTP.Modules.Ordering.Application.DTOs
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }

        public OrderItemType ItemType { get; set; }

        public Guid ProductId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public Guid? EsimPackageId { get; set; }

        public Guid? PhoneCardId { get; set; }

        public string ProductName { get; set; } = default!;

        public string? VariantName { get; set; }

        public string? Sku { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public string Currency { get; set; } = "VND";
    }
}
