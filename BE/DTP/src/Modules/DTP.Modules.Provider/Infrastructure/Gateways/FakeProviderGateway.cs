using DTP.Modules.Provider.Application.Abstractions.Gateways;
using DTP.Modules.Provider.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Gateways
{
    public class FakeProviderGateway : IProviderGateway
    {
        public Task<ProviderProvisionResultDto> CreateOrderAsync(
            ProviderCreateOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = new ProviderProvisionResultDto
            {
                Success = true,
                ProviderOrderCode = $"FAKE-{DateTime.UtcNow:yyyyMMddHHmmss}"
            };

            foreach (var item in request.Items)
            {
                result.Items.Add(new ProviderProvisionItemResultDto
                {
                    OrderItemId = item.OrderItemId,
                    Iccid = $"89884{Random.Shared.Next(10000000, 99999999)}",
                    Msisdn = null,
                    QrCodeUrl = $"https://fake-provider.test/qr/{Guid.NewGuid():N}",
                    QrCodeText = $"LPA:1$fake-provider.test${Guid.NewGuid():N}",
                    ActivationCode = Guid.NewGuid().ToString("N"),
                    ExpiredAt = DateTime.UtcNow.AddDays(30)
                });
            }

            return Task.FromResult(result);
        }
    }
}
