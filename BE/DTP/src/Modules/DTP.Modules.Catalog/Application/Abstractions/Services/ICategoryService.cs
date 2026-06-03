using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface ICategoryService
    {

        Task<CategoryDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);


        Task ClearCategoryCacheAsync(
            CancellationToken cancellationToken = default);


        Task<PagedResultDto<CategoryDto>> GetPublicAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

    }
}
