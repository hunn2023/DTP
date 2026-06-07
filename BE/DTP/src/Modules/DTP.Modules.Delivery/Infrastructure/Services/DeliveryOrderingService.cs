using DTP.Modules.Delivery.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Infrastructure.Services
{
    public class DeliveryOrderingService : IDeliveryOrderingService
    {
        private readonly IMediator _mediator;

        public DeliveryOrderingService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<DeliveryOrderSnapshotDto?> GetOrderForDeliveryAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery
            {
                OrderId = orderId,
                IsAdmin = true
            }, cancellationToken);

            if (order == null)
                return null;

            return new DeliveryOrderSnapshotDto
            {
                OrderId = order.Id,
                OrderCode = order.OrderCode,
                UserId = order.UserId,
                CustomerEmail = order.CustomerEmail,
                CustomerName = order.CustomerName,
                Items = order.Items.Select(x => new DeliveryOrderItemSnapshotDto
                {
                    OrderItemId = x.Id,
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    EsimPackageId = x.EsimPackageId,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity
                }).ToList()
            };
        }

        public async Task MarkOrderDeliveredAsync(
            Guid orderId,
            string? note,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new UpdateOrderStatusCommand
            {
                OrderId = orderId,
                Status = OrderStatus.Delivered,
                Note = note
            }, cancellationToken);
        }
    }
}
