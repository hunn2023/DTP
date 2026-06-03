using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface ICountryService
    {
        Task<Guid> CreateAsync(
            string code,
            string name,
            string slug,
            string? flagUrl,
            int sortOrder,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Guid id,
            string code,
            string name,
            string slug,
            string? flagUrl,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<CountryDto>> GetPublicAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);


        Task<PagedResultDto<CountryDto>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
