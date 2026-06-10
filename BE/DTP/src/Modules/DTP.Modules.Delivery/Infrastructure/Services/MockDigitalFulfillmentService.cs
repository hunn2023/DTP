using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Application.DTOs;
using DTP.Shared.Application.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Infrastructure.Services
{
    public class MockDigitalFulfillmentService : IDigitalFulfillmentService
    {
        private readonly IOrderDeliveryReader _orderDeliveryReader;

        public MockDigitalFulfillmentService(IOrderDeliveryReader orderDeliveryReader)
        {
            _orderDeliveryReader = orderDeliveryReader;
        }

        public async Task<DigitalFulfillmentResultDto> FulfillAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderDeliveryReader.GetOrderForDeliveryAsync(
                orderId,
                cancellationToken);

            if (order == null)
            {
                return new DigitalFulfillmentResultDto
                {
                    Success = false,
                    ErrorMessage = "Không tìm thấy đơn hàng để giao eSIM."
                };
            }

            if (!order.IsPaid)
            {
                return new DigitalFulfillmentResultDto
                {
                    Success = false,
                    ErrorMessage = "Đơn hàng chưa thanh toán, không thể giao eSIM."
                };
            }

            var result = new DigitalFulfillmentResultDto
            {
                Success = true
            };

            foreach (var item in order.Items)
            {
                result.Items.Add(new DigitalFulfillmentItemDto
                {
                    OrderItemId = item.OrderItemId,
                    QrCodeUrl = "https://api.qrserver.com/v1/create-qr-code/?size=240x240&data=LPA:1$dtp-esim-test$activation-code-test",
                    ActivationCode = "LPA:1$dtp-esim-test$activation-code-test",
                    SerialNumber = $"ESIM-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    ProviderReference = $"MOCK-{Guid.NewGuid():N}",
                    RawData = "{\"source\":\"mock\"}"
                });
            }

            return result;
        }
    }
}
