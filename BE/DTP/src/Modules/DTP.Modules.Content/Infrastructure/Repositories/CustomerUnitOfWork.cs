using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Infrastructure.Repositories
{
    public class CustomerUnitOfWork : ICustomerUnitOfWork
    {
        private readonly CustomerDbContext _context;

        public CustomerUnitOfWork(CustomerDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
