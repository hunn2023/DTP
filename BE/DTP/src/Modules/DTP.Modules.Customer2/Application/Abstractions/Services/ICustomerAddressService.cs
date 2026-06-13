
using DTP.Modules.Customer.Application.Commands.CustomerAddresses;
using DTP.Modules.Customer.Application.DTOs;


namespace DTP.Modules.Customer.Application.Abstractions.Services
{
    public interface ICustomerAddressService
    {
        Task<Guid> CreateAsync(
            CreateCustomerAddressCommand command,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            UpdateCustomerAddressCommand command,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            DeleteCustomerAddressCommand command,
            CancellationToken cancellationToken = default);

        Task<bool> SetDefaultAsync(
            SetDefaultCustomerAddressCommand command,
            CancellationToken cancellationToken = default);

        Task<List<CustomerAddressDto>> GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task<CustomerAddressDto?> GetDefaultAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);
    }
}
