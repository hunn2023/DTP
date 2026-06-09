using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Gateways
{
    public interface IProviderGatewayFactory
    {
        IProviderGateway Create(string providerCode);
    }
}
