using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;
using DTP.Shared.Storage;
using Microsoft.AspNetCore.Http;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ICacheService _cacheService;
        private readonly IFileStorageService _fileStorageService;

        public CountryService(
            ICountryRepository countryRepository,
            ICacheService cacheService,
            IFileStorageService fileStorageService)
        {
            _countryRepository = countryRepository;
            _cacheService = cacheService;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<Guid>> CreateAsync(
            string code,
            string name,
            string slug,
            string? flagUrl,
            string? region,
            string? description,
            int sortOrder,
            bool isActive = true,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Result<Guid>.Failure("Vui lòng nhập mã quốc gia.");

            if (string.IsNullOrWhiteSpace(name))
                return Result<Guid>.Failure("Vui lòng nhập tên quốc gia.");

            if (string.IsNullOrWhiteSpace(slug))
                return Result<Guid>.Failure("Vui lòng nhập slug.");

            code = code.Trim().ToUpper();
            name = name.Trim();
            slug = slug.Trim().ToLower();
            flagUrl = string.IsNullOrWhiteSpace(flagUrl) ? null : flagUrl.Trim();
            region = string.IsNullOrWhiteSpace(region) ? null : region.Trim();
            description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

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
                region,
                description,
                sortOrder,
                isActive);

            await _countryRepository.AddAsync(
                country,
                cancellationToken);

            await _countryRepository.SaveChangesAsync(
                cancellationToken);

            await ClearRelatedCacheAsync(cancellationToken);

            return Result<Guid>.Success(country.Id);
        }

        public async Task<Result> UpdateAsync(
            Guid id,
            string code,
            string name,
            string slug,
            string? flagUrl,
            string? region,
            string? description,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result.Failure("Id quốc gia không hợp lệ.");

            if (string.IsNullOrWhiteSpace(code))
                return Result.Failure("Vui lòng nhập mã quốc gia.");

            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure("Vui lòng nhập tên quốc gia.");

            if (string.IsNullOrWhiteSpace(slug))
                return Result.Failure("Vui lòng nhập slug.");

            var country = await _countryRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (country == null)
                return Result.Failure("Không tìm thấy quốc gia.");

            code = code.Trim().ToUpper();
            name = name.Trim();
            slug = slug.Trim().ToLower();
            flagUrl = string.IsNullOrWhiteSpace(flagUrl) ? null : flagUrl.Trim();
            region = string.IsNullOrWhiteSpace(region) ? null : region.Trim();
            description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

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
                region,
                description,
                sortOrder,
                isActive);

            _countryRepository.Update(country);

            await _countryRepository.SaveChangesAsync(
                cancellationToken);

            await ClearRelatedCacheAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result.Failure("Id quốc gia không hợp lệ.");

            var country = await _countryRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (country == null)
                return Result.Failure("Không tìm thấy quốc gia.");

            _countryRepository.Remove(country);

            await _countryRepository.SaveChangesAsync(
                cancellationToken);

            await ClearRelatedCacheAsync(cancellationToken);

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
                return Result<PagedResultDto<CountryDto>>.Success(cachedData);

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
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var countries = await _countryRepository.GetPagedAsync(
                keyword,
                pageIndex,
                pageSize,
                cancellationToken);

            return Result<PagedResultDto<CountryDto>>.Success(countries);
        }

        public async Task<Result<CountryDto>> UploadFlagAsync(
            Guid countryId,
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            if (countryId == Guid.Empty)
                return Result<CountryDto>.Failure("Id quốc gia không hợp lệ.");

            if (file == null || file.Length == 0)
                return Result<CountryDto>.Failure("Vui lòng chọn file ảnh.");

            var country = await _countryRepository.GetByIdAsync(
                countryId,
                cancellationToken);

            if (country == null)
                return Result<CountryDto>.Failure("Không tìm thấy quốc gia.");

            var uploadResult = await _fileStorageService.UploadImageAsync(
                file,
                UploadFolders.CountryFlags,
                cancellationToken);

            country.UpdateFlag(
                flagUrl: uploadResult.Url,
                flagKey: uploadResult.Key);

            _countryRepository.Update(country);

            await _countryRepository.SaveChangesAsync(
                cancellationToken);

            await ClearRelatedCacheAsync(cancellationToken);

            return Result<CountryDto>.Success(MapToDto(country));
        }


        public async Task<Result<PagedResultDto<HomeCountryEsimDto>>> GetHomeCountriesAsync(
            string? region,
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = BuildHomeCountriesCacheKey(
                    region,
                    keyword,
                    pageIndex,
                    pageSize);


            var cachedResult = await _cacheService.GetAsync<PagedResultDto<HomeCountryEsimDto>>(
                  cacheKey,
                  cancellationToken);

            if (cachedResult != null)
                return Result<PagedResultDto<HomeCountryEsimDto>>.Success(cachedResult);


            var result = await _countryRepository.GetHomeCountriesAsync(
                   region,
                   keyword,
                   pageIndex,
                   pageSize,
                   cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(10),
                cancellationToken);

            return Result<PagedResultDto<HomeCountryEsimDto>>.Success(result);
        }


        private async Task ClearRelatedCacheAsync(
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveByPrefixAsync(
                CountryCacheKeys.Prefix,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                "catalog:countries:",
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                "catalog:esim:",
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                "catalog:phonecards:",
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                "catalog:products:",
                cancellationToken);
        }



        private static CountryDto MapToDto(Country country)
        {
            return new CountryDto
            {
                Id = country.Id,
                Code = country.Code,
                Name = country.Name,
                Slug = country.Slug,
                FlagUrl = country.FlagUrl,
                FlagKey = country.FlagKey,
                Region = country.Region,
                Description = country.Description,
                SortOrder = country.SortOrder,
                IsActive = country.IsActive
            };
        }


        private static string BuildHomeCountriesCacheKey(
               string? region,
               string? keyword,
               int pageIndex,
               int pageSize)
        {
            var normalizedRegion = string.IsNullOrWhiteSpace(region)
                ? "all"
                : region.Trim().ToLower();

            var normalizedKeyword = string.IsNullOrWhiteSpace(keyword)
                ? "none"
                : keyword.Trim().ToLower();

            return $"catalog:home-countries:region:{normalizedRegion}:keyword:{normalizedKeyword}:page:{pageIndex}:size:{pageSize}";
        }
    }
}