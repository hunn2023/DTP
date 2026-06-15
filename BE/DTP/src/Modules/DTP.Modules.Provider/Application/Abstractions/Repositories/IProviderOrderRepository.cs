using DTP.Modules.Provider.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Repositories
{
    public interface IProviderOrderRepository
    {
        Task<ProviderOrder?> GetByDtpOrderIdAsync(
            Guid dtpOrderId,
            CancellationToken cancellationToken = default);

        Task<ProviderOrder?> GetByProviderOrderPublicIdAsync(
            string providerOrderPublicId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProviderOrder providerOrder,
            CancellationToken cancellationToken = default);
    }
}
