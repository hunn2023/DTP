using DTP.Shared.Infrastructure.Persistence;
using Order = DTP.Modules.Ordering.Domain.Entities.Order;

namespace DTP.Modules.Ordering.Application.Abstractions.Repositories
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        Task<Order?> GetDetailByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Order?> GetByOrderCodeAsync(string orderCode, CancellationToken cancellationToken = default);

        IQueryable<Order> Query();
    }
}
