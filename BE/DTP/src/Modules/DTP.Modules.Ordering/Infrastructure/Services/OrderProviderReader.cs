using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Modules.Ordering.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Ordering.Infrastructure.Services
{
    public class OrderProviderReader : IOrderProviderReader
    {
        private readonly OrderingDbContext _dbContext;

        public OrderProviderReader(OrderingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OrderForProviderDto?> GetForProviderAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .Where(x => x.Id == orderId)
                .Select(x => new OrderForProviderDto
                {
                    OrderId = x.Id,
                    OrderCode = x.OrderCode,
                    UserId = x.CustomerId.Value,
                    CustomerEmail = x.CustomerEmail ?? "",
                    CustomerName = x.CustomerName,
                    CustomerPhone = x.CustomerPhone,
                    TotalAmount = x.TotalAmount,
                    CurrencyCode = x.Currency,
                    Items = x.Items.Select(i => new OrderItemForProviderDto
                    {
                        OrderItemId = i.Id,
                        ProductId = i.ProductId,
                        ProductVariantId = i.ProductVariantId,
                        EsimPackageId = i.EsimPackageId,
                        Sku = i.Sku ?? "",
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
