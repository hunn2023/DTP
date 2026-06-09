using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Ordering.Domain.Entities;
using DTP.Modules.Ordering.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Ordering.Infrastructure.Repositories
{
    public class OrderHistoryRepository : RepositoryBase<OrderHistory>,
        IOrderHistoryRepository
    {
        private readonly OrderingDbContext _dbContext;

        public OrderHistoryRepository(OrderingDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<OrderHistory>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return _dbContext.OrderHistories
                .AsNoTracking()
                .Where(x => x.OrderId == orderId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
