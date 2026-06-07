using DTP.Modules.Content.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.Abstractions.Repositories
{
    public interface ICustomerAddressRepository
    {
        Task<CustomerAddress?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<CustomerAddress>> GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task<CustomerAddress?> GetDefaultAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            CustomerAddress address,
            CancellationToken cancellationToken = default);

        void Update(CustomerAddress address);

        void Remove(CustomerAddress address);
    }
}
