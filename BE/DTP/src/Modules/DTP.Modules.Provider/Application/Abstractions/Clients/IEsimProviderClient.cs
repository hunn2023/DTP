using DTP.Modules.Provider.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Clients
{
    public interface IEsimProviderClient
    {
        Task<IReadOnlyList<ProviderPackageProductRemoteDto>> GetPackageProductsAsync(
            Domain.Entities.Provider provider,
            CancellationToken cancellationToken = default);

        Task<ProviderEsimProductRemoteDto> GetProductEsimAsync(
            Domain.Entities.Provider provider,
            string sku,
            CancellationToken cancellationToken = default);
    }
}
