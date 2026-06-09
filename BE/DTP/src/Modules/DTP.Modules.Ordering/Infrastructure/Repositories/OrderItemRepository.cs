using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Ordering.Domain.Entities;
using DTP.Modules.Ordering.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Ordering.Infrastructure.Repositories
{
    public class OrderItemRepository : RepositoryBase<OrderItem>,
        IOrderItemRepository
    {
        private readonly OrderingDbContext _dbContext;

        public OrderItemRepository(OrderingDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return _dbContext.OrderItems
                .AsNoTracking()
                .Where(x => x.OrderId == orderId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

    }
}
