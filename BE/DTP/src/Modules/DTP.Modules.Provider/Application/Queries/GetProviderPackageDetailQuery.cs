using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Queries
{
    public class GetProviderPackageDetailQuery : IRequest<ProviderPackageDetailDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetProviderPackageDetailQueryHandler
        : IRequestHandler<GetProviderPackageDetailQuery, ProviderPackageDetailDto?>
    {
        private readonly IProviderPackageProductRepository _packageRepository;
        private readonly IProviderProductMappingRepository _mappingRepository;

        public GetProviderPackageDetailQueryHandler(
            IProviderPackageProductRepository packageRepository,
            IProviderProductMappingRepository mappingRepository)
        {
            _packageRepository = packageRepository;
            _mappingRepository = mappingRepository;
        }

        public async Task<ProviderPackageDetailDto?> Handle(
            GetProviderPackageDetailQuery request,
            CancellationToken cancellationToken)
        {
            var package = await _packageRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (package is null)
                return null;

            var mapping = await _mappingRepository.GetByProviderSkuAsync(
                package.ProviderId,
                package.ProviderSku,
                cancellationToken);

            return new ProviderPackageDetailDto
            {
                Id = package.Id,
                ProviderId = package.ProviderId,
                ProviderSku = package.ProviderSku,
                ProviderProductId = package.ProviderProductId,
                Name = package.Name,
                Model = package.Model,
                Regional = package.Regional,
                RegionId = package.RegionId,
                Price = package.Price,
                CurrencyCode = package.CurrencyCode,
                SyncStatus = package.SyncStatus,
                ErrorMessage = package.ErrorMessage,
                LastSyncedAt = package.LastSyncedAt,
                RawPackageJson = package.RawPackageJson,
                RawDetailJson = package.RawDetailJson,
                Mapping = mapping is null
                    ? null
                    : new ProviderProductMappingDto
                    {
                        Id = mapping.Id,
                        ProductId = mapping.ProductId,
                        ProductVariantId = mapping.ProductVariantId,
                        ProductPriceId = mapping.ProductPriceId,
                        EsimPackageId = mapping.EsimPackageId,
                        MappingStatus = mapping.MappingStatus
                    }
            };
        }
    }
}