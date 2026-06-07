using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Infrastructure.Repositories
{
    public class CustomerAddressRepository : ICustomerAddressRepository
    {
        private readonly CustomerDbContext _context;

        public CustomerAddressRepository(CustomerDbContext context)
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

        public async Task AddAsync(
            CustomerAddress address,
            CancellationToken cancellationToken = default)
        {
            await _context.CustomerAddresses.AddAsync(address, cancellationToken);
        }

        public void Update(CustomerAddress address)
        {
            _context.CustomerAddresses.Update(address);
        }

        public void Remove(CustomerAddress address)
        {
            address.Delete();
            _context.CustomerAddresses.Update(address);
        }
    }
}
