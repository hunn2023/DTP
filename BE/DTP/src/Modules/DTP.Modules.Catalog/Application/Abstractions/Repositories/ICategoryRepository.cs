using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Persistence;

namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface ICategoryRepository : IRepositoryBase<Category>
    {
        Task<List<Category>> GetListAsync(
            string? keyword,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<CategoryDto>> GetPublicPagedAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
