using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Services
{
    public interface IProviderReservationService
    {
        Task<ProviderReservationResult> ReserveOrderAsync(
            Guid dtpOrderId,
            CancellationToken cancellationToken = default);

        Task<bool> IsReservationValidAsync(
            Guid dtpOrderId,
            CancellationToken cancellationToken = default);
    }

    public class ProviderReservationResult
    {
        public Guid ProviderOrderId { get; set; }

        public string ProviderOrderPublicId { get; set; } = default!;

        public DateTime ReservedUntil { get; set; }
    }
}
