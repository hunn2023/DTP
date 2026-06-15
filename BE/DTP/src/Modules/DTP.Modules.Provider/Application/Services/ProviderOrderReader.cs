using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTP.Modules.Provider.Application.DTOs;

namespace DTP.Modules.Provider.Application.Services
{
    public class ProviderOrderReader : IProviderOrderReader
    {
        private readonly IOrderProviderReader _orderProviderReader;

        public ProviderOrderReader(IOrderProviderReader orderProviderReader)
        {
            _orderProviderReader = orderProviderReader;
        }

        public async Task<ProviderOrderReadDto?> GetOrderForReservationAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderProviderReader.GetForProviderAsync(
                orderId,
                cancellationToken);

            if (order is null)
            {
                return null;
            }

            return new ProviderOrderReadDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                UserId = order.UserId,
                CustomerEmail = order.CustomerEmail,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                TotalAmount = order.TotalAmount,
                CurrencyCode = order.CurrencyCode,
                Items = order.Items.Select(i => new ProviderOrderItemReadDto
                {
                    OrderItemId = i.OrderItemId,
                    ProductId = i.ProductId,
                    ProductVariantId = i.ProductVariantId,
                    EsimPackageId = i.EsimPackageId,
                    Sku = i.Sku,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }
    }
}
