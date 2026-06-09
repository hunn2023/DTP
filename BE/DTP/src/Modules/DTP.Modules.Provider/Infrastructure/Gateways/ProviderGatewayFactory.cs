using DTP.Modules.Provider.Application.Abstractions.Gateways;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Gateways
{
    public class ProviderGatewayFactory : IProviderGatewayFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ProviderGatewayFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IProviderGateway Create(string providerCode)
        {
            providerCode = providerCode.Trim().ToUpperInvariant();

            return providerCode switch
            {
                "FAKE" => _serviceProvider.GetRequiredService<FakeProviderGateway>(),

                // Sau này thêm:
                // "AIRALO" => _serviceProvider.GetRequiredService<AiraloProviderGateway>(),
                // "TELNA" => _serviceProvider.GetRequiredService<TelnaProviderGateway>(),

                _ => throw new Exception($"Provider gateway not supported: {providerCode}")
            };
        }
    }
}
