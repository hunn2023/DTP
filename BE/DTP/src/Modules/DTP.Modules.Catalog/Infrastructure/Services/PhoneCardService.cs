using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.Commands.PhoneCards;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;


namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class PhoneCardService : IPhoneCardService
    {
        private readonly IPhoneCardRepository _phoneCardRepository;
        private readonly ICacheService _cacheService;

        public PhoneCardService(
            IPhoneCardRepository phoneCardRepository,
            ICacheService cacheService)
        {
            _phoneCardRepository = phoneCardRepository;
            _cacheService = cacheService;
        }

        public async Task<PagedResultDto<PhoneCardDto>> GetPublicPagedAsync(
            Guid? providerId,
            decimal? minFaceValue,
            decimal? maxFaceValue,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var cacheKey = PhoneCardCacheKeys.PublicPaged(
                providerId,
                minFaceValue,
                maxFaceValue,
                pageIndex,
                pageSize);

            var cachedData = await _cacheService.GetAsync<PagedResultDto<PhoneCardDto>>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return cachedData;
            }

            var result = await _phoneCardRepository.GetPublicPagedAsync(
                providerId,
                minFaceValue,
                maxFaceValue,
                pageIndex,
                pageSize,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(1),
                cancellationToken);

            return result;
        }

        public async Task<PhoneCardDto?> GetPublicBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            slug = slug.Trim().ToLower();

            var cacheKey = PhoneCardCacheKeys.PublicBySlug(slug);

            var cachedData = await _cacheService.GetAsync<PhoneCardDto>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return cachedData;
            }

            var result = await _phoneCardRepository.GetPublicBySlugAsync(
                slug,
                cancellationToken);

            if (result is not null)
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    result,
                    TimeSpan.FromHours(1),
                    cancellationToken);
            }

            return result;
        }

        public async Task<PagedResultDto<PhoneCardDto>> GetPagedAsync(
            string? keyword,
            Guid? productVariantId,
            Guid? providerId,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            return await _phoneCardRepository.GetPagedAsync(
                keyword,
                productVariantId,
                providerId,
                isActive,
                pageIndex,
                pageSize,
                cancellationToken);
        }

        public async Task<PhoneCardDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _phoneCardRepository.GetByIdDtoAsync(
                id,
                cancellationToken);
        }

        public async Task<Guid> CreateAsync(
            CreatePhoneCardCommand command,
            CancellationToken cancellationToken = default)
        {
            var id = await _phoneCardRepository.CreateAsync(
                command,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                PhoneCardCacheKeys.PublicPrefix,
                cancellationToken);

            return id;
        }

        public async Task<bool> UpdateAsync(
            UpdatePhoneCardCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _phoneCardRepository.UpdateAsync(
                command,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                PhoneCardCacheKeys.PublicPrefix,
                cancellationToken);

            return result;
        }

        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _phoneCardRepository.DeleteAsync(
                id,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                PhoneCardCacheKeys.PublicPrefix,
                cancellationToken);

            return result;
        }
    }
}
