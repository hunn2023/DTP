using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Commands.Providers
{
    public class ActivateSyncedProviderPackageCommand : IRequest<bool>
    {
        public Guid ProviderPackageProductId { get; set; }
    }

    public class ActivateSyncedProviderPackageCommandHandler
        : IRequestHandler<ActivateSyncedProviderPackageCommand, bool>
    {
        private readonly IProviderPackageProductRepository _packageRepository;
        private readonly IProviderProductMappingRepository _mappingRepository;
        private readonly ICatalogProductProvisioningService _catalogProvisioningService;
        private readonly IProviderUnitOfWork _unitOfWork;

        public ActivateSyncedProviderPackageCommandHandler(
            IProviderPackageProductRepository packageRepository,
            IProviderProductMappingRepository mappingRepository,
            ICatalogProductProvisioningService catalogProvisioningService,
            IProviderUnitOfWork unitOfWork)
        {
            _packageRepository = packageRepository;
            _mappingRepository = mappingRepository;
            _catalogProvisioningService = catalogProvisioningService;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            ActivateSyncedProviderPackageCommand request,
            CancellationToken cancellationToken)
        {
            const int pageSize = 100;

            var allItems = new List<ProviderPackageProduct>();

            for (var pageIndex = 1; ; pageIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var (items, total) = await _packageRepository.GetPagedAsync(
                    providerId: null,
                    keyword: null,
                    syncStatus: "Provisioned",
                    pageIndex: pageIndex,
                    pageSize: pageSize,
                    cancellationToken: cancellationToken);

                if (items.Count == 0)
                    break;

                allItems.AddRange(items);

                if (allItems.Count >= total)
                    break;
            }

            foreach (var item in allItems)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var mapping = await _mappingRepository.GetByProviderSkuAsync(
                    item.ProviderId,
                    item.ProviderSku,
                    cancellationToken);

                if (mapping is null)
                {
                    throw new InvalidOperationException(
                        $"Không tìm thấy mapping Catalog cho ProviderId={item.ProviderId}, " +
                        $"ProviderSku={item.ProviderSku}.");
                }

                await _catalogProvisioningService.ActivateProviderProvisionedProductAsync(
                    mapping.ProductId,
                    mapping.ProductVariantId,
                    mapping.ProductPriceId,
                    mapping.EsimPackageId,
                    cancellationToken);

                mapping.Activate();
                item.MarkActivated();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
