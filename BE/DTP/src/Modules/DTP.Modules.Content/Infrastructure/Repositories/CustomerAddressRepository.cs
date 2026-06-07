
using DTP.Modules.Customer.Application.Abstractions.Repositories;
using DTP.Modules.Customer.Domain.Entities;
using DTP.Modules.Customer.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Customer.Infrastructure.Repositories
{
    public class CustomerAddressRepository : RepositoryBase<CustomerAddress>,
     ICustomerAddressRepository
    {
        private readonly CustomerDbContext _context;

        public CustomerAddressRepository(CustomerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CustomerAddress?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.CustomerAddresses
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<List<CustomerAddress>> GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.CustomerAddresses
                .Where(x => x.CustomerId == customerId && !x.IsDeleted)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<CustomerAddress?> GetDefaultAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.CustomerAddresses
                .FirstOrDefaultAsync(
                    x => x.CustomerId == customerId &&
                         x.IsDefault &&
                         !x.IsDeleted,
                    cancellationToken);
        }

    }
}
