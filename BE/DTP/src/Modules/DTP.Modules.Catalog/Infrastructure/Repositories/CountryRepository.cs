using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Catalog.Infrastructure.Repositories
{
    public class CountryRepository
     : RepositoryBase<Country>, ICountryRepository
    {
        private readonly CatalogDbContext _context;

        public CountryRepository(CatalogDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Country>> GetActiveListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Countries
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Countries.AsQueryable();

            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return await query.AnyAsync(
                x => x.Code == code,
                cancellationToken);
        }


        public async Task<Country?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Countries
             .AsNoTracking()
             .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        }

        public async Task<bool> ExistsBySlugAsync(
            string slug,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Countries.AsQueryable();

            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return await query.AnyAsync(
                x => x.Slug == slug,
                cancellationToken);
        }


        public async Task<PagedResultDto<CountryDto>> GetPublicPagedAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Countries
                .AsNoTracking()
                .Where(x => x.IsActive);

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CountryDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Slug = x.Slug,
                    FlagUrl = x.FlagUrl,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<CountryDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }


        public async Task<PagedResultDto<CountryDto>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Countries
                .Where(x => !x.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Name.Contains(keyword) ||
                    x.Code!.Contains(keyword) ||
                    x.Region!.Contains(keyword));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                 .Where(x => !x.IsDeleted)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CountryDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Slug = x.Slug,
                    FlagUrl = x.FlagUrl,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive,
                    Region = x.Region,
                    Description = x.Description,
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<CountryDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDto<HomeCountryEsimDto>> GetHomeCountriesAsync(
            string? region,
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 12;

            if (pageSize > 100)
                pageSize = 100;

            var normalizedRegion = string.IsNullOrWhiteSpace(region)
                ? "all"
                : region.Trim().ToLower();

            var normalizedKeyword = string.IsNullOrWhiteSpace(keyword)
                ? null
                : keyword.Trim().ToLower();

            var now = DateTime.Now;

            var query = _context.Countries
                .Where(x => x.IsActive && !x.IsDeleted)
                .AsNoTracking();


            if (normalizedRegion != "all")
            {
                query = query.Where(x =>
                    x.Region != null &&
                    x.Region.ToLower() == normalizedRegion);
            }

             if (!string.IsNullOrWhiteSpace(normalizedKeyword))
            {
                query = query.Where(x =>
                    x.Name.ToLower().Contains(normalizedKeyword) ||
                    x.Code.ToLower().Contains(normalizedKeyword) ||
                    x.Slug.ToLower().Contains(normalizedKeyword));
            }

            var projectedQuery = query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Select(country => new HomeCountryEsimDto
                {
                    CountryId = country.Id,
                    Code = country.Code,
                    Name = country.Name,
                    Slug = country.Slug,
                    FlagUrl = country.FlagUrl,
                    Region = country.Region,

                    PackageCount = _context.EsimPackages
                        .Count(pkg =>
                            pkg.CountryId == country.Id &&
                            pkg.IsActive && !pkg.IsDeleted),

                    PriceFrom = _context.ProductPrices
                        .Where(price =>
                            price.IsActive &&
                            !price.IsDeleted &&
                            price.SalePrice > 0 &&
                            (price.StartDate == null || price.StartDate <= now) &&
                            (price.EndDate == null || price.EndDate >= now) &&
                            _context.Products.Any(product =>
                                product.Id == price.ProductId &&
                                product.CountryId == country.Id &&
                                product.IsActive))
                        .OrderBy(price => price.SalePrice)
                        .Select(price => price.SalePrice)
                        .FirstOrDefault(),

                    Currency = _context.ProductPrices
                        .Where(price =>
                            price.IsActive &&
                            price.SalePrice > 0 &&
                            (price.StartDate == null || price.StartDate <= now) &&
                            (price.EndDate == null || price.EndDate >= now) &&
                            _context.Products.Any(product =>
                                product.Id == price.ProductId &&
                                product.CountryId == country.Id &&
                                product.IsActive))
                        .OrderBy(price => price.SalePrice)
                        .Select(price => price.Currency)
                        .FirstOrDefault() ?? "VND",

                    IsHot = _context.Products.Any(product =>
                        product.CountryId == country.Id &&
                        product.IsActive &&
                        product.IsHot)
                })
                .Where(x => x.PackageCount > 0 && x.PriceFrom > 0);

            var totalItems = await projectedQuery.CountAsync(cancellationToken);

            var items = await projectedQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<HomeCountryEsimDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
