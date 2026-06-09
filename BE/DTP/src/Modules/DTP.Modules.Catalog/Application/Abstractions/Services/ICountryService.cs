using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using Microsoft.AspNetCore.Http;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface ICountryService
    {
        Task<Result<Guid>> CreateAsync(
            string code,
            string name,
            string slug,
            string? flagUrl,
            string? region,
            string? description,
            int sortOrder,
            bool isActive = true,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            Guid id,
            string code,
            string name,
            string slug,
            string? flagUrl,
            string? region,
            string? description,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<PagedResultDto<CountryDto>>> GetPublicAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result<PagedResultDto<CountryDto>>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result<CountryDto>> UploadFlagAsync(
            Guid countryId,
            IFormFile file,
            CancellationToken cancellationToken = default);
    }
}
