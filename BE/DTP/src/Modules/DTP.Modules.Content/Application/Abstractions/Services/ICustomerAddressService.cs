using DTP.Modules.Content.Application.Commands.CustomerAddresses;
using DTP.Modules.Content.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
