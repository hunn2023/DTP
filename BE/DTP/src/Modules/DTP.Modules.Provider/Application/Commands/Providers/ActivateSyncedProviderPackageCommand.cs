using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var package = await _packageRepository.GetByIdAsync(
                request.ProviderPackageProductId,
                cancellationToken);

            if (package is null)
                throw new InvalidOperationException("Provider package không tồn tại.");

            if (package.SyncStatus != "Provisioned" && package.SyncStatus != "Activated")
                throw new InvalidOperationException("Package chưa được provision sang Catalog.");

            var mapping = await _mappingRepository.GetByProviderSkuAsync(
                package.ProviderId,
                package.ProviderSku,
                cancellationToken);

            if (mapping is null)
                throw new InvalidOperationException("Không tìm thấy mapping Catalog.");

            await _catalogProvisioningService.ActivateProviderProvisionedProductAsync(
                mapping.ProductId,
                mapping.ProductVariantId,
                mapping.ProductPriceId,
                mapping.EsimPackageId,
                cancellationToken);

            mapping.Activate();
            package.MarkActivated();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
