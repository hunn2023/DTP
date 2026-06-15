using DTP.Modules.Ordering.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.Abstractions.Services
{
    public interface IOrderProviderReader
    {
        Task<OrderForProviderDto?> GetForProviderAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}
