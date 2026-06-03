using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Commands.EsimPackages;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Catalog.Infrastructure.Repositories
{
    public class EsimPackageRepository
         : RepositoryBase<EsimPackage>,
           IEsimPackageRepository
    {
        private readonly CatalogDbContext _context;

        public EsimPackageRepository(CatalogDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<EsimPackageDto>> GetPublicPagedAsync(
            Guid? countryId,
            Guid? carrierId,
            bool? isUnlimited,
            int? validityDays,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.EsimPackages
                .AsNoTracking()
                .Where(x => x.IsActive)
                .AsQueryable();

            query = ApplyFilters(
                query,
                keyword: null,
                productVariantId: null,
                countryId,
                carrierId,
                isUnlimited,
                validityDays,
                isActive: null);

            return await ToPagedDtoAsync(
                query,
                pageIndex,
                pageSize,
                cancellationToken);
        }

        public async Task<EsimPackageDto?> GetPublicBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            slug = slug.Trim().ToLower();

            return await _context.EsimPackages
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.Slug == slug)
                .Select(x => new EsimPackageDto
                {
                    Id = x.Id,
                    ProductVariantId = x.ProductVariantId,
                    ProductVariantName = x.ProductVariant.Name,
                    CountryId = x.CountryId,
                    CountryName = x.Country.Name,
                    CarrierId = x.CarrierId,
                    CarrierName = x.Carrier.Name,
                    Name = x.Name,
                    Slug = x.Slug,
                    DataAmount = x.DataAmount,
                    DataUnit = x.DataUnit,
                    ValidityDays = x.ValidityDays,
                    Price = x.Price,
                    Currency = x.Currency,
                    IsUnlimited = x.IsUnlimited,
                    IsActive = x.IsActive,
                    SortOrder = x.SortOrder
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedResultDto<EsimPackageDto>> GetPagedAsync(
            string? keyword,
            Guid? productVariantId,
            Guid? countryId,
            Guid? carrierId,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.EsimPackages
                .AsNoTracking()
                .AsQueryable();

            query = ApplyFilters(
                query,
                keyword,
                productVariantId,
                countryId,
                carrierId,
                isUnlimited: null,
                validityDays: null,
                isActive);

            return await ToPagedDtoAsync(
                query,
                pageIndex,
                pageSize,
                cancellationToken);
        }

        public async Task<EsimPackageDto?> GetByIdDtoAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.EsimPackages
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new EsimPackageDto
                {
                    Id = x.Id,
                    ProductVariantId = x.ProductVariantId,
                    ProductVariantName = x.ProductVariant.Name,
                    CountryId = x.CountryId,
                    CountryName = x.Country.Name,
                    CarrierId = x.CarrierId,
                    CarrierName = x.Carrier.Name,
                    Name = x.Name,
                    Slug = x.Slug,
                    DataAmount = x.DataAmount,
                    DataUnit = x.DataUnit,
                    ValidityDays = x.ValidityDays,
                    Price = x.Price,
                    Currency = x.Currency,
                    IsUnlimited = x.IsUnlimited,
                    IsActive = x.IsActive,
                    SortOrder = x.SortOrder
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Guid> CreateAsync(
            CreateEsimPackageCommand command,
            CancellationToken cancellationToken = default)
        {
            var esimPackage = new EsimPackage(
                command.ProductVariantId,
                command.CountryId,
                command.CarrierId,
                command.Name,
                command.Slug,
                command.DataAmount,
                command.DataUnit,
                command.ValidityDays,
                command.Price,
                command.Currency,
                command.IsUnlimited,
                command.SortOrder);

            if (!command.IsActive)
            {
                esimPackage.Update(
                    command.CountryId,
                    command.CarrierId,
                    command.Name,
                    command.Slug,
                    command.DataAmount,
                    command.DataUnit,
                    command.ValidityDays,
                    command.Price,
                    command.Currency,
                    command.IsUnlimited,
                    command.SortOrder,
                    false);
            }

            _context.EsimPackages.Add(esimPackage);

            await _context.SaveChangesAsync(cancellationToken);

            return esimPackage.Id;
        }

        public async Task<bool> UpdateAsync(
            UpdateEsimPackageCommand command,
            CancellationToken cancellationToken = default)
        {
            var esimPackage = await _context.EsimPackages
                .FirstOrDefaultAsync(
                    x => x.Id == command.Id,
                    cancellationToken);

            if (esimPackage is null)
            {
                return false;
            }

            esimPackage.Update(
                command.CountryId,
                command.CarrierId,
                command.Name,
                command.Slug,
                command.DataAmount,
                command.DataUnit,
                command.ValidityDays,
                command.Price,
                command.Currency,
                command.IsUnlimited,
                command.SortOrder,
                command.IsActive);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var esimPackage = await _context.EsimPackages
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (esimPackage is null)
            {
                return false;
            }

            _context.EsimPackages.Remove(esimPackage);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static IQueryable<EsimPackage> ApplyFilters(
            IQueryable<EsimPackage> query,
            string? keyword,
            Guid? productVariantId,
            Guid? countryId,
            Guid? carrierId,
            bool? isUnlimited,
            int? validityDays,
            bool? isActive)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Name.Contains(keyword) ||
                    x.Slug.Contains(keyword) ||
                    x.Country.Name.Contains(keyword) ||
                    x.Carrier.Name.Contains(keyword));
            }

            if (productVariantId.HasValue)
            {
                query = query.Where(x =>
                    x.ProductVariantId == productVariantId.Value);
            }

            if (countryId.HasValue)
            {
                query = query.Where(x =>
                    x.CountryId == countryId.Value);
            }

            if (carrierId.HasValue)
            {
                query = query.Where(x =>
                    x.CarrierId == carrierId.Value);
            }

            if (isUnlimited.HasValue)
            {
                query = query.Where(x =>
                    x.IsUnlimited == isUnlimited.Value);
            }

            if (validityDays.HasValue)
            {
                query = query.Where(x =>
                    x.ValidityDays == validityDays.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(x =>
                    x.IsActive == isActive.Value);
            }

            return query;
        }

        private static async Task<PagedResultDto<EsimPackageDto>> ToPagedDtoAsync(
            IQueryable<EsimPackage> query,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Price)
                .ThenBy(x => x.ValidityDays)
                .Select(x => new EsimPackageDto
                {
                    Id = x.Id,
                    ProductVariantId = x.ProductVariantId,
                    ProductVariantName = x.ProductVariant.Name,
                    CountryId = x.CountryId,
                    CountryName = x.Country.Name,
                    CarrierId = x.CarrierId,
                    CarrierName = x.Carrier.Name,
                    Name = x.Name,
                    Slug = x.Slug,
                    DataAmount = x.DataAmount,
                    DataUnit = x.DataUnit,
                    ValidityDays = x.ValidityDays,
                    Price = x.Price,
                    Currency = x.Currency,
                    IsUnlimited = x.IsUnlimited,
                    IsActive = x.IsActive,
                    SortOrder = x.SortOrder
                })
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<EsimPackageDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
