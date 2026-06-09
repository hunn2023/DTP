using DTP.Modules.Ordering.Infrastructure.Persistence;
using DTP.Shared.Application.Delivery;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Ordering.Infrastructure.Services
{
    public class OrderDeliveryReader : IOrderDeliveryReader
    {
        private readonly OrderingDbContext _dbContext;

        public OrderDeliveryReader(OrderingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DeliveryOrderInfoDto?> GetOrderForDeliveryAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

            if (order == null)
                return null;

            return new DeliveryOrderInfoDto
            {
                OrderId = order.Id,
                OrderCode = order.OrderCode,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,

                // Nếu hiện tại chỉ bán eSIM thì để Esim.
                DeliveryType = SharedDeliveryType.Esim,

                // Sửa theo enum/status thật của Order của bạn.
                IsPaid = order.PaymentStatus.ToString() == "Paid",

                Items = order.Items.Select(x => new DeliveryOrderItemDto
                {
                    OrderItemId = x.Id,
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    ProductName = x.ProductName,
                    Sku = x.Sku,
                    Quantity = x.Quantity
                }).ToList()
            };
        }
    }
}
