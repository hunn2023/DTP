using DTP.Modules.Ordering.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.Abstractions.Repositories
{
    public interface IOrderHistoryRepository : IRepositoryBase<OrderHistory>
    {
        Task<List<OrderHistory>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    }
}
