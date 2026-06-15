using DTP.Modules.Provider.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Services
{
    public interface ICatalogProductProvisioningService
    {
        Task<ProvisionProviderEsimProductResult> ProvisionProviderEsimProductAsync(
            ProvisionProviderEsimProductRequest request,
            CancellationToken cancellationToken = default);

        Task ActivateProviderProvisionedProductAsync(
            Guid productId,
            Guid productVariantId,
            Guid? productPriceId,
            Guid esimPackageId,
            CancellationToken cancellationToken = default);
    }
}
