using DTP.Modules.Provider.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Repositories
{
    public interface IProviderOrderItemRepository
    {
        Task<ProviderOrderItem?> GetByProviderOrderAndSkuAsync(
            Guid providerOrderId,
            string sku,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProviderOrderItem item,
            CancellationToken cancellationToken = default);
    }
}
