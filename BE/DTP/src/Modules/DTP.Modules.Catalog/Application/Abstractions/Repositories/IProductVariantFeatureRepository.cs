using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface IProductVariantFeatureRepository : IRepositoryBase<ProductVariantFeature>
    {
        Task<List<ProductVariantFeature>> GetByProductVariantIdAsync(
            Guid productVariantId,
            CancellationToken cancellationToken = default);

        Task<int> GetNextSortOrderAsync(
            Guid productVariantId,
            CancellationToken cancellationToken = default);
    }
}
