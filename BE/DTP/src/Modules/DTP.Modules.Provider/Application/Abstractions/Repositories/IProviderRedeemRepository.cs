using DTP.Modules.Provider.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Repositories
{
    public interface IProviderRedeemRepository
    {
        Task<ProviderRedeem?> GetBySerialAsync(
            string serial,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ProviderRedeem>> GetPendingAsync(
            int take,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ProviderRedeem>> GetDoneNotEmailSentAsync(
            int take,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProviderRedeem redeem,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Guid>> GetOrderIdsReadyForDeliveryAsync(
            int take,
            CancellationToken cancellationToken = default);

        Task MarkEmailSentByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ProviderRedeem>> GetDoneNotEmailSentByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}
