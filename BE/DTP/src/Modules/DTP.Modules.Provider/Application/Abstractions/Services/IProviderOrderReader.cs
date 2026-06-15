using DTP.Modules.Provider.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Services
{
    public interface IProviderOrderReader
    {
        Task<ProviderOrderReadDto?> GetOrderForReservationAsync(
             Guid orderId,
             CancellationToken cancellationToken = default);
    }
}
