using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Ordering.Domain.Entities;
using DTP.Modules.Ordering.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order>,
        IOrderRepository
    {
        private readonly OrderingDbContext _context;

        public OrderRepository(OrderingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order?> GetDetailByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(x => x.Items)
                .Include(x => x.Histories)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<Order?> GetByOrderCodeAsync(
            string orderCode,
            CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(x => x.Items)
                .Include(x => x.Histories)
                .FirstOrDefaultAsync(x => x.OrderCode == orderCode && !x.IsDeleted, cancellationToken);
        }

        public IQueryable<Order> Query()
        {
            return _context.Orders.AsQueryable();
        }
    }
}
