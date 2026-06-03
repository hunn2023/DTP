using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProductCacheInvalidator
    {
        Task ClearAllProductCacheAsync(CancellationToken cancellationToken = default);

        Task ClearProductDetailAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

        Task ClearRelatedCatalogCacheAsync(
        CancellationToken cancellationToken = default);
    }
}
