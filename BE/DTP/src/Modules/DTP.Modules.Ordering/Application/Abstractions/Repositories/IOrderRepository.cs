using DTP.Modules.Ordering.Domain.Enums;
using DTP.Shared.Infrastructure.Persistence;
using Order = DTP.Modules.Ordering.Domain.Entities.Order;

namespace DTP.Modules.Ordering.Application.Abstractions.Repositories
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        Task<Order?> GetDetailByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Order?> GetByCodeAsync(string orderCode, CancellationToken cancellationToken = default);

        Task<(List<Order> Items, int Total)> GetPagedAsync(
            string? keyword,
            Guid? customerId,
            OrderStatus? status,
            OrderPaymentStatus? paymentStatus,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<List<Order>> GetByCustomerIdAsync(
            Guid customerId,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
