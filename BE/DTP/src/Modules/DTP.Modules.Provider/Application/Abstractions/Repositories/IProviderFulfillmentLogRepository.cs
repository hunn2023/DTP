using DTP.Modules.Provider.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Repositories
{
    public interface IProviderFulfillmentLogRepository
    {
        Task AddAsync(
            ProviderFulfillmentLog log,
            CancellationToken cancellationToken = default);

        Task<ProviderFulfillmentLog?> GetByOrderItemIdAsync(
            Guid orderItemId,
            CancellationToken cancellationToken = default);
    }
}
