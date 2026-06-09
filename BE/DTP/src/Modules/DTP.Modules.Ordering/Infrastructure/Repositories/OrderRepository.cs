using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Ordering.Domain.Entities;
using DTP.Modules.Ordering.Domain.Enums;
using DTP.Modules.Ordering.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order>,
        IOrderRepository
    {
        private readonly OrderingDbContext _dbContext;

        public OrderRepository(OrderingDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }



        public Task<Order?> GetDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _dbContext.Orders
                .Include(x => x.Items)
                .Include(x => x.Histories)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public Task<Order?> GetByCodeAsync(string orderCode, CancellationToken cancellationToken = default)
        {
            return _dbContext.Orders
                .FirstOrDefaultAsync(x => x.OrderCode == orderCode, cancellationToken);
        }

        public async Task<(List<Order> Items, int Total)> GetPagedAsync(
            string? keyword,
            Guid? customerId,
            OrderStatus? status,
            OrderPaymentStatus? paymentStatus,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Orders.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.OrderCode.Contains(keyword) ||
                    x.CustomerEmail!.Contains(keyword) ||
                    x.CustomerPhone!.Contains(keyword) ||
                    x.CustomerName!.Contains(keyword));
            }

            if (customerId.HasValue)
            {
                query = query.Where(x => x.CustomerId == customerId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (paymentStatus.HasValue)
            {
                query = query.Where(x => x.PaymentStatus == paymentStatus.Value);
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public Task<List<Order>> GetByCustomerIdAsync(
            Guid customerId,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.Orders
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
    }
}
