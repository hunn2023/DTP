using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProviderService
    {
        Task<List<ProviderDto>> GetActiveAsync(CancellationToken cancellationToken = default);
       
        Task<ProviderDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

        Task ClearProviderCacheAsync( CancellationToken cancellationToken = default);

        Task<PagedResultDto<ProviderDto>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
