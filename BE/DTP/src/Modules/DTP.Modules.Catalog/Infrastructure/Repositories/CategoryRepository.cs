using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Catalog.Infrastructure.Repositories
{
    public class CategoryRepository
    : RepositoryBase<Category>,
      ICategoryRepository
    {
        private readonly CatalogDbContext _context;

        public CategoryRepository(CatalogDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetListAsync(
            string? keyword,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Categories
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Name.Contains(keyword) ||
                    (x.Code != null && x.Code.Contains(keyword)));
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(
            string name,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Categories
                .AsNoTracking()
                .Where(x => x.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }



        public async Task<bool> ExistsByCodeAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Categories
               .AsNoTracking()
               .Where(x => x.Code == code && !x.IsDeleted);
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }
            return await query.AnyAsync(cancellationToken);
        }


        public async Task<bool> ExistsBySlugAsync(
           string slug,
           Guid? excludeId = null,
           CancellationToken cancellationToken = default)
        {
            var query = _context.Categories
               .AsNoTracking()
               .Where(x => x.Slug == slug && !x.IsDeleted);
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }
            return await query.AnyAsync(cancellationToken);
        }



        public async Task<PagedResultDto<CategoryDto>> GetPublicPagedAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 10;

            if (pageSize > 100)
                pageSize = 100;

            var query = _context.Categories
                .Where(x => !x.IsDeleted)
                .AsNoTracking();


            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Slug = x.Slug,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<CategoryDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }


        public async Task<Category?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == code && !x.IsDeleted, cancellationToken);
        }
    }
}
