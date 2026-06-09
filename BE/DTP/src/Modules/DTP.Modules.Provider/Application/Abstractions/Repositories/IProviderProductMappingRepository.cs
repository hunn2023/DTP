using DTP.Modules.Provider.Application.DTOs;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Modules.Provider.Domain.Enums;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Repositories
{
    public interface IProviderProductMappingRepository
    {
        Task<ProviderProductMapping?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<ProviderProductMapping?> GetActiveMappingAsync(
            ProviderProductType productType,
            Guid productId,
            Guid productVariantId,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<ProviderProductMappingDto>> GetPagedAsync(
            Guid? providerId,
            ProviderProductType? productType,
            Guid? productId,
            Guid? productVariantId,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsMappingAsync(
            Guid providerId,
            Guid productVariantId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProviderProductMapping mapping,
            CancellationToken cancellationToken = default);

        void Update(ProviderProductMapping mapping);

        void Remove(ProviderProductMapping mapping);
    }
}
