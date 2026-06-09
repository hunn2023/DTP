using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Abstractions.Services
{
    public interface IDeliveryOrderingService
    {
        Task<DeliveryOrderSnapshotDto?> GetOrderForDeliveryAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task MarkOrderDeliveredAsync(
            Guid orderId,
            string? note,
            CancellationToken cancellationToken = default);
    }

    public class DeliveryOrderSnapshotDto
    {
        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public Guid UserId { get; set; }

        public string CustomerEmail { get; set; } = default!;

        public string? CustomerName { get; set; }

        public List<DeliveryOrderItemSnapshotDto> Items { get; set; } = new();
    }

    public class DeliveryOrderItemSnapshotDto
    {
        public Guid OrderItemId { get; set; }

        public Guid ProductId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public Guid? EsimPackageId { get; set; }

        public string ProductName { get; set; } = default!;

        public int Quantity { get; set; }
    }
}
