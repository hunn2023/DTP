using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.Abstractions.Services
{
    public interface IOrderRateLimitService
    {
        Task<bool> IsCreateOrderBlockedAsync(
            Guid userId,
            string ipAddress,
            CancellationToken cancellationToken = default);

        Task RegisterCreateOrderAttemptAsync(
            Guid userId,
            string ipAddress,
            CancellationToken cancellationToken = default);
    }
}
