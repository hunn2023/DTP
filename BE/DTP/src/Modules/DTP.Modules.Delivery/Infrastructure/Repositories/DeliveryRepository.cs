using DTP.Modules.Delivery.Application.Abstractions.Repositories;
using DTP.Modules.Delivery.Domain.Enums;
using DTP.Modules.Delivery.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Delivery.Infrastructure.Repositories
{
    public class DeliveryRepository : RepositoryBase<Domain.Entities.Delivery>,
         IDeliveryRepository
    {
        private readonly DeliveryDbContext _dbContext;

        public DeliveryRepository(DeliveryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Domain.Entities.Delivery?> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Deliveries
                .Include(x => x.Items)
                .Include(x => x.Histories)
                .FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
        }

        public async Task<bool> ExistsByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Deliveries
                .AnyAsync(x => x.OrderId == orderId, cancellationToken);
        }

        public async Task<(List<Domain.Entities.Delivery> Items, int Total)> GetPagedAsync(
            string? keyword,
            DeliveryStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Deliveries
                .Include(x => x.Items)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.OrderCode.Contains(keyword) ||
                    x.CustomerEmail!.Contains(keyword) ||
                    x.CustomerName!.Contains(keyword));
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }


        public async Task<Domain.Entities.Delivery?> GetTrackingByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Deliveries
     .Include(x => x.Items)
     .Include(x => x.Histories)
     .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
