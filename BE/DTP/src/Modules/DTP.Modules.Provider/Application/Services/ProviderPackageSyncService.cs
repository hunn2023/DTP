using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Clients;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Services
{
    public class ProviderPackageSyncService : IProviderPackageSyncService
    {
        private readonly IProviderRepository _providerRepository;
        private readonly IProviderPackageProductRepository _packageRepository;
        private readonly IProviderProductMappingRepository _mappingRepository;
        private readonly IProviderApiLogRepository _apiLogRepository;
        private readonly IProviderUnitOfWork _unitOfWork;
        private readonly IEsimProviderClient _esimProviderClient;
        private readonly ICatalogProductProvisioningService _catalogProvisioningService;

        public ProviderPackageSyncService(
            IProviderRepository providerRepository,
            IProviderPackageProductRepository packageRepository,
            IProviderProductMappingRepository mappingRepository,
            IProviderApiLogRepository apiLogRepository,
            IProviderUnitOfWork unitOfWork,
            IEsimProviderClient esimProviderClient,
            ICatalogProductProvisioningService catalogProvisioningService)
        {
            _providerRepository = providerRepository;
            _packageRepository = packageRepository;
            _mappingRepository = mappingRepository;
            _apiLogRepository = apiLogRepository;
            _unitOfWork = unitOfWork;
            _esimProviderClient = esimProviderClient;
            _catalogProvisioningService = catalogProvisioningService;
        }

        public async Task<Result<ProviderPackageSyncResult>> SyncAsync(
            string providerCode,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(providerCode))
                return Result<ProviderPackageSyncResult>.Failure("ProviderCode không được rỗng.");


            var provider = await _providerRepository.GetByCodeAsync(
                providerCode,
                cancellationToken);

            if (provider is null)
                return Result<ProviderPackageSyncResult>.Failure("Provider không tồn tại.");


            if (!provider.IsActive)
                return Result<ProviderPackageSyncResult>.Failure("Provider đang inactive.");


            var result = new ProviderPackageSyncResult();

            IReadOnlyList<ProviderPackageProductRemoteDto> remotePackages;

            try
            {
                remotePackages = await _esimProviderClient.GetPackageProductsAsync(
                    provider,
                    cancellationToken);

                await _apiLogRepository.AddAsync(
                    new ProviderApiLog(
                        provider.Id,
                        "GET PACKAGE PRODUCT",
                        "GET",
                        null,
                        $"Total: {remotePackages.Count}",
                        200,
                        true,
                        null),
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _apiLogRepository.AddAsync(
                    new ProviderApiLog(
                        provider.Id,
                        "GET PACKAGE PRODUCT",
                        "GET",
                        null,
                        null,
                        null,
                        false,
                        ex.Message),
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                
                return Result<ProviderPackageSyncResult>.Failure($"Lỗi khi gọi API provider: {ex.Message}");
            }

            result.Total = remotePackages.Count;

            foreach (var remotePackage in remotePackages)
            {
                try
                {
                    await UpsertProviderPackageAsync(
                        provider,
                        remotePackage,
                        cancellationToken);

                    result.Synced++;
                }
                catch (Exception ex)
                {
                    result.Failed++;
                    result.Errors.Add($"{remotePackage.Sku}: {ex.Message}");
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            foreach (var remotePackage in remotePackages)
            {
                try
                {
                    var package = await _packageRepository.GetByProviderSkuAsync(
                        provider.Id,
                        remotePackage.Sku,
                        cancellationToken);

                    if (package is null)
                        continue;

                    var detail = await _esimProviderClient.GetProductEsimAsync(
                        provider,
                        remotePackage.Sku,
                        cancellationToken);

                    package.MarkDetailSynced(detail.RawJson);

                    var provisionRequest = BuildProvisionRequest(
                        provider,
                        package,
                        detail);

                    var provisionResult = await _catalogProvisioningService
                        .ProvisionProviderEsimProductAsync(
                            provisionRequest,
                            cancellationToken);

                    await UpsertMappingAsync(
                        provider,
                        package,
                        provisionResult,
                        cancellationToken);

                    package.MarkProvisioned();

                    result.Provisioned++;

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    result.Failed++;
                    result.Errors.Add($"{remotePackage.Sku}: {ex.Message}");

                    var package = await _packageRepository.GetByProviderSkuAsync(
                        provider.Id,
                        remotePackage.Sku,
                        cancellationToken);

                    package?.MarkFailed(ex.Message);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            return Result<ProviderPackageSyncResult>.Success(result);

        }

        private async Task<Result> UpsertProviderPackageAsync(
            Domain.Entities.Provider provider,
            ProviderPackageProductRemoteDto remotePackage,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(remotePackage.Sku))
                return Result.Failure("SKU của package không được rỗng.");

            var existing = await _packageRepository.GetByProviderSkuAsync(
                provider.Id,
                remotePackage.Sku,
                cancellationToken);

            if (existing is null)
            {
                var package = new ProviderPackageProduct(
                    provider.Id,
                    remotePackage.Sku,
                    remotePackage.Id,
                    remotePackage.Name,
                    remotePackage.Model,
                    remotePackage.Regional,
                    remotePackage.RegionId,
                    remotePackage.Price,
                    remotePackage.CurrencyCode,
                    remotePackage.ImageUrl,
                    remotePackage.RawJson);

                await _packageRepository.AddAsync(package, cancellationToken);
            }
            else
            {
                existing.UpdatePackageInfo(
                    remotePackage.Id,
                    remotePackage.Name,
                    remotePackage.Model,
                    remotePackage.Regional,
                    remotePackage.RegionId,
                    remotePackage.Price,
                    remotePackage.CurrencyCode,
                    remotePackage.ImageUrl,
                    remotePackage.RawJson);
            }
            return Result.Success();
        }

        private async Task<Result> UpsertMappingAsync(
            Domain.Entities.Provider provider,
            ProviderPackageProduct package,
            ProvisionProviderEsimProductResult provisionResult,
            CancellationToken cancellationToken)
        {
            var existing = await _mappingRepository.GetByProviderSkuAsync(
                provider.Id,
                package.ProviderSku,
                cancellationToken);

            if (existing is null)
            {
                var mapping = new ProviderProductMapping(
                    provider.Id,
                    package.ProviderSku,
                    package.ProviderProductId,
                    provisionResult.ProductId,
                    provisionResult.ProductVariantId,
                    provisionResult.ProductPriceId,
                    provisionResult.EsimPackageId);

                await _mappingRepository.AddAsync(mapping, cancellationToken);
            }
            else
            {
                existing.UpdateCatalogMapping(
                    provisionResult.ProductId,
                    provisionResult.ProductVariantId,
                    provisionResult.ProductPriceId,
                    provisionResult.EsimPackageId);
            }
            return Result.Success();
        }

        private static ProvisionProviderEsimProductRequest BuildProvisionRequest(
            Domain.Entities.Provider provider,
            ProviderPackageProduct package,
            ProviderEsimProductRemoteDto detail)
        {
            var dataText = detail.IsUnlimited
                ? "Unlimited"
                : $"{detail.DataAmount}{detail.DataUnit}";

            var variantName = $"{dataText} - {detail.ValidityDays} ngày";

            return new ProvisionProviderEsimProductRequest
            {
                ProviderId = provider.Id,
                ProviderCode = provider.Code,
                ProviderSku = package.ProviderSku,
                ProviderProductId = package.ProviderProductId,

                ProductName = package.Name,
                ProductDescription = detail.CoverageDescription,

                VariantName = variantName,
                VariantSku = package.ProviderSku,

                Price = package.Price > 0 ? package.Price : detail.Price,
                CurrencyCode = package.CurrencyCode,

                DataAmount = detail.DataAmount,
                DataUnit = detail.DataUnit,
                ValidityDays = detail.ValidityDays,
                IsUnlimited = detail.IsUnlimited,

                CoverageType = detail.CoverageType,
                CoverageDescription = detail.CoverageDescription,

                Countries = detail.Countries
                    .Select(x => new ProvisionCountryDto
                    {
                        CountryCode = x.CountryCode,
                        CountryName = x.CountryName
                    })
                    .ToList(),

                Operators = detail.Operators,

                IsActive = false
            };
        }
    }
}
