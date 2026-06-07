using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ICacheService _cacheService;

        public CountryService(
            ICountryRepository countryRepository,
            ICacheService cacheService)
        {
            _countryRepository = countryRepository;
            _cacheService = cacheService;
        }

        public async Task<Result<Guid>> CreateAsync(
            string code,
            string name,
            string slug,
            string? flagUrl,
            int sortOrder,
            CancellationToken cancellationToken = default)
        {
            code = code.Trim().ToUpper();
            slug = slug.Trim();

            var existsCode = await _countryRepository.ExistsByCodeAsync(
                code,
                null,
                cancellationToken);

            if (existsCode)
                return Result<Guid>.Failure("Mã quốc gia đã tồn tại.");


            var existsSlug = await _countryRepository.ExistsBySlugAsync(
                slug,
                null,
                cancellationToken);

            if (existsSlug)
                return Result<Guid>.Failure("Slug đã tồn tại.");

            var country = new Country(

                code,
                name,
                slug,
                flagUrl,
                sortOrder);

            await _countryRepository.AddAsync(
                country,
                cancellationToken);

            await _countryRepository.SaveChangesAsync(
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                CountryCacheKeys.Prefix,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync("catalog:countries:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:esim:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:phonecards:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:products:", cancellationToken);

            return Result<Guid>.Success(country.Id);
        }

        public async Task<Result> UpdateAsync(
            Guid id,
            string code,
            string name,
            string slug,
            string? flagUrl,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (country == null)
                return Result.Failure("Không tìm thấy quốc gia.");


            code = code.Trim().ToUpper();
            slug = slug.Trim();

            var existsCode = await _countryRepository.ExistsByCodeAsync(
                code,
                id,
                cancellationToken);

            if (existsCode)
                return Result.Failure("Mã quốc gia đã tồn tại.");


            var existsSlug = await _countryRepository.ExistsBySlugAsync(
                slug,
                id,
                cancellationToken);

            if (existsSlug)
                return Result.Failure("Slug đã tồn tại.");


            country.Update(
                code,
                name,
                slug,
                flagUrl,
                sortOrder,
                isActive);

            await _countryRepository.SaveChangesAsync(
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                CountryCacheKeys.Prefix,
                cancellationToken);


            await _cacheService.RemoveByPrefixAsync("catalog:countries:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:esim:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:phonecards:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:products:", cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (country == null)
                return Result.Failure("Không tìm thấy quốc gia.");


            _countryRepository.Remove(country);

            await _countryRepository.SaveChangesAsync(
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                CountryCacheKeys.Prefix,
                cancellationToken);


            await _cacheService.RemoveByPrefixAsync("catalog:countries:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:esim:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:phonecards:", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("catalog:products:", cancellationToken);

            return Result.Success();
        }


        public async Task<Result<PagedResultDto<CountryDto>>> GetPublicAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var cacheKey = CountryCacheKeys.PublicActivePaged(
                pageIndex,
                pageSize);

            var cachedData = await _cacheService.GetAsync<PagedResultDto<CountryDto>>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return Result<PagedResultDto<CountryDto>>.Success(cachedData);
            }

            var result = await _countryRepository.GetPublicPagedAsync(
                pageIndex,
                pageSize,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(6),
                cancellationToken);

            return Result<PagedResultDto<CountryDto>>.Success(result);
        }


        public async Task<Result<PagedResultDto<CountryDto>>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {

            var countries = await _countryRepository.GetPagedAsync(
                keyword,
                pageIndex,
                pageSize,
                cancellationToken);

            return Result<PagedResultDto<CountryDto>>.Success(countries);
        }
    }
}
