using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.Commands.CustomerAddresses;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.Services
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerAddressRepository _addressRepository;
        private readonly ICustomerUnitOfWork _unitOfWork;

        public CustomerAddressService(
            ICustomerRepository customerRepository,
            ICustomerAddressRepository addressRepository,
            ICustomerUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _addressRepository = addressRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateAsync(
            CreateCustomerAddressCommand command,
            CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetByIdAsync(
                command.CustomerId,
                cancellationToken);

            if (customer == null)
                throw new Exception("Customer not found.");

            if (string.IsNullOrWhiteSpace(command.ReceiverName))
                throw new ArgumentException("Receiver name is required.");

            if (string.IsNullOrWhiteSpace(command.PhoneNumber))
                throw new ArgumentException("Phone number is required.");

            if (string.IsNullOrWhiteSpace(command.AddressLine))
                throw new ArgumentException("Address line is required.");

            var existingAddresses = await _addressRepository.GetByCustomerIdAsync(
                command.CustomerId,
                cancellationToken);

            var shouldSetDefault = command.IsDefault || !existingAddresses.Any();

            if (shouldSetDefault)
            {
                foreach (var item in existingAddresses)
                {
                    item.UnsetDefault();
                    _addressRepository.Update(item);
                }
            }

            var address = new CustomerAddress(
                command.CustomerId,
                command.ReceiverName,
                command.PhoneNumber,
                command.AddressLine,
                command.Ward,
                command.District,
                command.City,
                command.CountryCode,
                shouldSetDefault);

            await _addressRepository.AddAsync(address, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return address.Id;
        }

        public async Task<bool> UpdateAsync(
            UpdateCustomerAddressCommand command,
            CancellationToken cancellationToken = default)
        {
            var address = await _addressRepository.GetByIdAsync(
                command.Id,
                cancellationToken);

            if (address == null)
                throw new Exception("Customer address not found.");

            if (string.IsNullOrWhiteSpace(command.ReceiverName))
                throw new ArgumentException("Receiver name is required.");

            if (string.IsNullOrWhiteSpace(command.PhoneNumber))
                throw new ArgumentException("Phone number is required.");

            if (string.IsNullOrWhiteSpace(command.AddressLine))
                throw new ArgumentException("Address line is required.");

            address.Update(
                command.ReceiverName,
                command.PhoneNumber,
                command.AddressLine,
                command.Ward,
                command.District,
                command.City,
                command.CountryCode);

            _addressRepository.Update(address);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            DeleteCustomerAddressCommand command,
            CancellationToken cancellationToken = default)
        {
            var address = await _addressRepository.GetByIdAsync(
                command.Id,
                cancellationToken);

            if (address == null)
                throw new Exception("Customer address not found.");

            var wasDefault = address.IsDefault;
            var customerId = address.CustomerId;

            _addressRepository.Remove(address);

            if (wasDefault)
            {
                var addresses = await _addressRepository.GetByCustomerIdAsync(
                    customerId,
                    cancellationToken);

                var nextDefault = addresses
                    .Where(x => x.Id != address.Id)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefault();

                if (nextDefault != null)
                {
                    nextDefault.SetDefault();
                    _addressRepository.Update(nextDefault);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> SetDefaultAsync(
            SetDefaultCustomerAddressCommand command,
            CancellationToken cancellationToken = default)
        {
            var addresses = await _addressRepository.GetByCustomerIdAsync(
                command.CustomerId,
                cancellationToken);

            var selectedAddress = addresses.FirstOrDefault(x => x.Id == command.AddressId);

            if (selectedAddress == null)
                throw new Exception("Customer address not found.");

            foreach (var address in addresses)
            {
                if (address.Id == command.AddressId)
                    address.SetDefault();
                else
                    address.UnsetDefault();

                _addressRepository.Update(address);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<CustomerAddressDto>> GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var addresses = await _addressRepository.GetByCustomerIdAsync(
                customerId,
                cancellationToken);

            return addresses
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedAt)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<CustomerAddressDto?> GetDefaultAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var address = await _addressRepository.GetDefaultAsync(
                customerId,
                cancellationToken);

            return address == null ? null : MapToDto(address);
        }

        private static CustomerAddressDto MapToDto(CustomerAddress address)
        {
            return new CustomerAddressDto
            {
                Id = address.Id,
                CustomerId = address.CustomerId,
                ReceiverName = address.ReceiverName,
                PhoneNumber = address.PhoneNumber,
                AddressLine = address.AddressLine,
                Ward = address.Ward,
                District = address.District,
                City = address.City,
                CountryCode = address.CountryCode,
                IsDefault = address.IsDefault
            };
        }
    }
}
