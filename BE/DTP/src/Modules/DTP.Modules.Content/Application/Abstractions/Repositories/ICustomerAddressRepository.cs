
using DTP.Modules.Customer.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;

namespace DTP.Modules.Customer.Application.Abstractions.Repositories
{
    public interface ICustomerAddressRepository : IRepositoryBase<CustomerAddress>
    {

        Task<List<CustomerAddress>> GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task<CustomerAddress?> GetDefaultAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);

    }
}
