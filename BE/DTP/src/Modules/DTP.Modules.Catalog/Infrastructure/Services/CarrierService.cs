using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class CarrierService : ICarrierService
    {
        private readonly ICarrierRepository _carrierRepository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public CarrierService(ICarrierRepository carrierRepository, ICatalogUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _carrierRepository = carrierRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Guid> CreateAsync(
            string? code,
            string name,
            string slug,
            Guid countryId,
            string? logoUrl,
            int sortOrder,
            CancellationToken cancellationToken = default)
        {
            if (countryId == Guid.Empty)
                throw new Exception("Vui lòng chọn quốc gia.");

            var existsName = await _carrierRepository.ExistsByNameAsync(
                name,
                countryId,
                null,
                cancellationToken);

            if (existsName)
                throw new Exception("Nhà mạng đã tồn tại trong quốc gia này.");

            var existsSlug = await _carrierRepository.ExistsBySlugAsync(
                slug,
                null,
                cancellationToken);

            if (existsSlug)
                throw new Exception("Slug đã tồn tại.");

            var carrier = new Carrier(
                code,
                name,
                slug,
                countryId,
                logoUrl,
                sortOrder);

            await _carrierRepository.AddAsync(carrier, cancellationToken);
            await _carrierRepository.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
              CarrierCacheKeys.Prefix,
              cancellationToken);

            await _cacheService.RemoveByPrefixAsync("catalog:carriers:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:esim:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:products:", cancellationToken);


            return carrier.Id;
        }

        public async Task UpdateAsync(
            Guid id,
            string? code,
            string name,
            string slug,
            Guid countryId,
            string? logoUrl,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var carrier = await _carrierRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (carrier == null)
                throw new Exception("Không tìm thấy nhà mạng.");

            if (countryId == Guid.Empty)
                throw new Exception("Vui lòng chọn quốc gia.");

            var existsName = await _carrierRepository.ExistsByNameAsync(
                name,
                countryId,
                id,
                cancellationToken);

            if (existsName)
                throw new Exception("Nhà mạng đã tồn tại trong quốc gia này.");

            var existsSlug = await _carrierRepository.ExistsBySlugAsync(
                slug,
                id,
                cancellationToken);

            if (existsSlug)
                throw new Exception("Slug đã tồn tại.");

            carrier.Update(
                code,
                name,
                slug,
                countryId,
                logoUrl,
                sortOrder,
                isActive);

            await _carrierRepository.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
              CarrierCacheKeys.Prefix,
              cancellationToken);

            await _cacheService.RemoveByPrefixAsync("catalog:carriers:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:esim:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:products:", cancellationToken);
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var carrier = await _carrierRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (carrier == null)
                throw new Exception("Không tìm thấy nhà mạng.");

            _carrierRepository.Remove(carrier);

            await _carrierRepository.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
              CarrierCacheKeys.Prefix,
              cancellationToken);

            await _cacheService.RemoveByPrefixAsync("catalog:carriers:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:esim:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:products:", cancellationToken);
        }


        public async Task<List<CarrierDto>> GetActiveAsync(
        CancellationToken cancellationToken = default)
        {
            var cacheKey = CarrierCacheKeys.ActiveList;

            var cachedData = await _cacheService.GetAsync<List<CarrierDto>>(
                cacheKey,
                cancellationToken);

            if (cachedData != null)
                return cachedData;

            var carriers = await _carrierRepository.GetListAsync(
                keyword: null,
                null,
                cancellationToken);

            var result = carriers
                .Where(x => x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
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
                .ToList();

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(12),
                cancellationToken);

            return result;
        }

        public async Task<CarrierDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        {
            var cacheKey = CarrierCacheKeys.Detail(id);

            var cachedData = await _cacheService.GetAsync<CarrierDto>(
                cacheKey,
                cancellationToken);

            if (cachedData != null)
                return cachedData;

            var carrier = await _carrierRepository.GetByIdAsync(id, cancellationToken);

            if (carrier == null)
                return null;

            var result = new CarrierDto
            {
                Id = carrier.Id,
                Code = carrier.Code,
                Name = carrier.Name,
                Slug = carrier.Slug,
                LogoUrl = carrier.LogoUrl,
                SortOrder = carrier.SortOrder,
                IsActive = carrier.IsActive
            };

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(12),
                cancellationToken);

            return result;
        }


        public async Task ClearCarrierCacheAsync(
             CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveByPrefixAsync(
                CarrierCacheKeys.Prefix,
                cancellationToken);
        }


        public async Task<PagedResultDto<CarrierDto>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            return await _carrierRepository.GetPagedAsync(
                keyword,
                pageIndex,
                pageSize,
                cancellationToken);
        }

        public async Task<PagedResultDto<CarrierDto>> GetPublicAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var cacheKey = CarrierCacheKeys.PublicActivePaged(
                pageIndex,
                pageSize);

            var cachedData = await _cacheService.GetAsync<PagedResultDto<CarrierDto>>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return cachedData;
            }

            var result = await _carrierRepository.GetPublicPagedAsync(
                pageIndex,
                pageSize,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(6),
                cancellationToken);

            return result;
        }
    }
}
