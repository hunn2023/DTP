using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface ICategoryService
    {

        Task<Result<CategoryDto>> CreateAsync(
                   string? code,
                   string name,
                   string slug,
                   int sortOrder,
                   CancellationToken cancellationToken = default);


        Task<Result<CategoryDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);


        Task ClearCategoryCacheAsync(
            CancellationToken cancellationToken = default);


        Task<Result<PagedResultDto<CategoryDto>>> GetPublicAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);


        Task<Result> DeleteAsync(Guid Id, CancellationToken cancellationToken = default);


        Task<Result<CategoryDto>> UpdateAsync(
                Guid id,
                string? code,
                string name,
                string slug,
                int sortOrder,
                CancellationToken cancellationToken = default);
    }
}
