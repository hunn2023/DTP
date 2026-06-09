using DTP.Modules.Delivery.Domain.Enums;
using DTP.Shared.Infrastructure.Persistence;


namespace DTP.Modules.Delivery.Application.Abstractions.Repositories
{
    public interface IDeliveryRepository : IRepositoryBase<Domain.Entities.Delivery>
    {
     
        Task<Domain.Entities.Delivery?> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task<(List<Domain.Entities.Delivery> Items, int Total)> GetPagedAsync(
            string? keyword,
            DeliveryStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

    }
}
