using DTP.Modules.Provider.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Repositories
{
    public interface IProviderProductMappingRepository : IRepositoryBase<ProviderProductMapping>
    {
        Task<ProviderProductMapping?> GetByProviderSkuAsync(
            Guid providerId,
            string providerSku,
            CancellationToken cancellationToken = default);

        Task<ProviderProductMapping?> GetByEsimPackageIdAsync(
            Guid esimPackageId,
            CancellationToken cancellationToken = default);
    }
}
