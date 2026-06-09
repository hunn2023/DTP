using DTP.Modules.Catalog.Application.Abstractions.Repositories;
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

                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,

                    ProductVariantId = x.ProductVariantId,
                    ProductVariantName = x.ProductVariant.Name,

                    ProviderId = x.ProviderId,
                    ProviderName = x.Provider.Name,

                    CountryId = x.CountryId,
                    CountryName = x.Country.Name,

                    Name = x.Name,
                    Slug = x.Slug,
                    ProviderPackageCode = x.ProviderPackageCode,

                    DataAmount = x.DataAmount,
                    DataUnit = x.DataUnit,
                    ValidityDays = x.ValidityDays,
                    IsUnlimited = x.IsUnlimited,

                    CoverageType = x.CoverageType,
                    CoverageDescription = x.CoverageDescription,
                    ActivationPolicy = x.ActivationPolicy,
                    SpeedPolicy = x.SpeedPolicy,

                    HotspotSupported = x.HotspotSupported,
                    PhoneNumberSupported = x.PhoneNumberSupported,
                    SmsSupported = x.SmsSupported,
                    KycRequired = x.KycRequired,
                    QrDeliveryType = x.QrDeliveryType,

                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive,

                    Carriers = x.Carriers
                        .OrderBy(c => c.Carrier.Name)
                        .Select(c => new EsimPackageCarrierDto
                        {
                            CarrierId = c.CarrierId,
                            CarrierName = c.Carrier.Name
                        })
                        .ToList()
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

                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,

                    ProductVariantId = x.ProductVariantId,
                    ProductVariantName = x.ProductVariant.Name,

                    ProviderId = x.ProviderId,
                    ProviderName = x.Provider.Name,

                    CountryId = x.CountryId,
                    CountryName = x.Country.Name,

                    Name = x.Name,
                    Slug = x.Slug,
                    ProviderPackageCode = x.ProviderPackageCode,

                    DataAmount = x.DataAmount,
                    DataUnit = x.DataUnit,
                    ValidityDays = x.ValidityDays,
                    IsUnlimited = x.IsUnlimited,

                    CoverageType = x.CoverageType,
                    CoverageDescription = x.CoverageDescription,
                    ActivationPolicy = x.ActivationPolicy,
                    SpeedPolicy = x.SpeedPolicy,

                    HotspotSupported = x.HotspotSupported,
                    PhoneNumberSupported = x.PhoneNumberSupported,
                    SmsSupported = x.SmsSupported,
                    KycRequired = x.KycRequired,
                    QrDeliveryType = x.QrDeliveryType,

                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive,

                    Carriers = x.Carriers
                        .OrderBy(c => c.Carrier.Name)
                        .Select(c => new EsimPackageCarrierDto
                        {
                            CarrierId = c.CarrierId,
                            CarrierName = c.Carrier.Name
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsSlugAsync(
            string slug,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            slug = slug.Trim().ToLower();

            var query = _context.EsimPackages
                .AsNoTracking()
                .Where(x => x.Slug == slug);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> ExistsProviderPackageCodeAsync(
            Guid providerId,
            string providerPackageCode,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            providerPackageCode = providerPackageCode.Trim();

            var query = _context.EsimPackages
                .AsNoTracking()
                .Where(x =>
                    x.ProviderId == providerId &&
                    x.ProviderPackageCode == providerPackageCode);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken);
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
                    x.ProviderPackageCode.Contains(keyword) ||
                    x.Product.Name.Contains(keyword) ||
                    x.ProductVariant.Name.Contains(keyword) ||
                    x.Provider.Name.Contains(keyword) ||
                    x.Country.Name.Contains(keyword) ||
                    x.Carriers.Any(c => c.Carrier.Name.Contains(keyword)));
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
                    x.Carriers.Any(c => c.CarrierId == carrierId.Value));
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
                .ThenBy(x => x.Country.Name)
                .ThenBy(x => x.ValidityDays)
                .ThenBy(x => x.DataAmount)
                .Select(x => new EsimPackageDto
                {
                    Id = x.Id,

                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,

                    ProductVariantId = x.ProductVariantId,
                    ProductVariantName = x.ProductVariant.Name,

                    ProviderId = x.ProviderId,
                    ProviderName = x.Provider.Name,

                    CountryId = x.CountryId,
                    CountryName = x.Country.Name,

                    Name = x.Name,
                    Slug = x.Slug,
                    ProviderPackageCode = x.ProviderPackageCode,

                    DataAmount = x.DataAmount,
                    DataUnit = x.DataUnit,
                    ValidityDays = x.ValidityDays,
                    IsUnlimited = x.IsUnlimited,

                    CoverageType = x.CoverageType,
                    CoverageDescription = x.CoverageDescription,
                    ActivationPolicy = x.ActivationPolicy,
                    SpeedPolicy = x.SpeedPolicy,

                    HotspotSupported = x.HotspotSupported,
                    PhoneNumberSupported = x.PhoneNumberSupported,
                    SmsSupported = x.SmsSupported,
                    KycRequired = x.KycRequired,
                    QrDeliveryType = x.QrDeliveryType,

                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive,

                    Carriers = x.Carriers
                        .OrderBy(c => c.Carrier.Name)
                        .Select(c => new EsimPackageCarrierDto
                        {
                            CarrierId = c.CarrierId,
                            CarrierName = c.Carrier.Name
                        })
                        .ToList()
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

        public async Task<List<EsimPackage>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.EsimPackages
                .AsNoTracking()
                .Include(x => x.Provider)
                .Include(x => x.Country)
                .Include(x => x.ProductVariant)
                .Include(x => x.Carriers)
                    .ThenInclude(x => x.Carrier)
                .Where(x => x.ProductId == productId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(cancellationToken);
        }
    }
}