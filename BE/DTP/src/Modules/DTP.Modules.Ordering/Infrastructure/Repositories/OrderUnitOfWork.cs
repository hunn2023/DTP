using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Ordering.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Infrastructure.Repositories
{
    public class OrderUnitOfWork : IOrderUnitOfWork
    {
        private readonly OrderingDbContext _context;

        public OrderUnitOfWork(OrderingDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
