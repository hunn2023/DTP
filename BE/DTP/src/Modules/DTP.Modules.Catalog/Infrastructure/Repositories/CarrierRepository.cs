using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Catalog.Infrastructure.Repositories
{
    public class CarrierRepository
      : RepositoryBase<Carrier>,
        ICarrierRepository
    {
        private readonly CatalogDbContext _context;

        public CarrierRepository(CatalogDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Carrier>> GetListAsync(
            string? keyword,
            Guid? countryId,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Carriers
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Name.Contains(keyword) ||
                    x.Slug.Contains(keyword) ||
                    (x.Code != null && x.Code.Contains(keyword)));
            }

            if (countryId.HasValue)
            {
                query = query.Where(x => x.CountryId == countryId.Value);
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(
            string name,
            Guid countryId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            name = name.Trim();

            var query = _context.Carriers
                .AsNoTracking()
                .Where(x =>
                    x.Name == name &&
                    x.CountryId == countryId);

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
            slug = slug.Trim();

            var query = _context.Carriers
                .AsNoTracking()
                .Where(x => x.Slug == slug);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }


        public async Task<PagedResultDto<CarrierDto>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Carriers
                .Where(x => !x.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Name.Contains(keyword) ||
                    x.Code!.Contains(keyword));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CarrierDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Slug = x.Slug,
                    LogoUrl = x.LogoUrl,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive,
                    CountryId = x.CountryId
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<CarrierDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }


        public async Task<PagedResultDto<CarrierDto>> GetPublicPagedAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Carriers
                .Where(x => x.IsActive && !x.IsDeleted)
                .AsNoTracking()
                .Where(x => x.IsActive);

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CarrierDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Slug = x.Slug,
                    LogoUrl = x.LogoUrl,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<CarrierDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
