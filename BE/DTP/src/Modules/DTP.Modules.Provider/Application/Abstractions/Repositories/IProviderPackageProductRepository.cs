using DTP.Modules.Provider.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Repositories
{
    public interface IProviderPackageProductRepository
    {
        Task<ProviderPackageProduct?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<ProviderPackageProduct?> GetByProviderSkuAsync(
            Guid providerId,
            string providerSku,
            CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<ProviderPackageProduct> Items, int Total)> GetPagedAsync(
            Guid? providerId,
            string? keyword,
            string? syncStatus,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProviderPackageProduct entity,
            CancellationToken cancellationToken = default);
    }
}
