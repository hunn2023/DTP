using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.Commands.Customers;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerUnitOfWork _unitOfWork;

        public CustomerService(
            ICustomerRepository customerRepository,
            ICustomerUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateAsync(
            CreateCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.UserId == Guid.Empty)
                throw new ArgumentException("UserId is required.");

            if (string.IsNullOrWhiteSpace(command.Email))
                throw new ArgumentException("Email is required.");

            var email = command.Email.Trim().ToLower();

            var existsUser = await _customerRepository.ExistsByUserIdAsync(
                command.UserId,
                null,
                cancellationToken);

            if (existsUser)
                throw new Exception("Customer already exists for this user.");

            var existsEmail = await _customerRepository.ExistsByEmailAsync(
                email,
                null,
                cancellationToken);

            if (existsEmail)
                throw new Exception("Customer email already exists.");

            var customer = new Customer(
                command.UserId,
                email,
                command.FullName,
                command.PhoneNumber);

            await _customerRepository.AddAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return customer.Id;
        }

        public async Task<bool> UpdateAsync(
            UpdateCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetByIdAsync(
                command.Id,
                cancellationToken);

            if (customer == null)
                throw new Exception("Customer not found.");

            if (string.IsNullOrWhiteSpace(command.Email))
                throw new ArgumentException("Email is required.");

            var email = command.Email.Trim().ToLower();

            var existsEmail = await _customerRepository.ExistsByEmailAsync(
                email,
                customer.Id,
                cancellationToken);

            if (existsEmail)
                throw new Exception("Customer email already exists.");

            customer.UpdateAdminInfo(
                email,
                command.FullName,
                command.PhoneNumber,
                command.DateOfBirth,
                command.AvatarUrl,
                command.Note);

            _customerRepository.Update(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            DeleteCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetByIdAsync(
                command.Id,
                cancellationToken);

            if (customer == null)
                throw new Exception("Customer not found.");

            _customerRepository.Remove(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> ChangeStatusAsync(
            ChangeCustomerStatusCommand command,
            CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetByIdAsync(
                command.Id,
                cancellationToken);

            if (customer == null)
                throw new Exception("Customer not found.");

            customer.ChangeStatus(command.Status);

            _customerRepository.Update(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<CustomerDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetByIdAsync(
                id,
                cancellationToken);

            return customer == null ? null : MapToDto(customer);
        }

        public async Task<CustomerDto?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetByUserIdAsync(
                userId,
                cancellationToken);

            return customer == null ? null : MapToDto(customer);
        }

        public async Task<CustomerDetailDto?> GetDetailAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetDetailByIdAsync(
                id,
                cancellationToken);

            if (customer == null)
                return null;

            return new CustomerDetailDto
            {
                Id = customer.Id,
                UserId = customer.UserId,
                Email = customer.Email,
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber,
                DateOfBirth = customer.DateOfBirth,
                AvatarUrl = customer.AvatarUrl,
                Note = customer.Note,
                Status = customer.Status,
                LastLoginAt = customer.LastLoginAt,
                CreatedAt = customer.CreatedAt,
                Addresses = customer.Addresses
                    .Where(x => !x.IsDeleted)
                    .OrderByDescending(x => x.IsDefault)
                    .ThenByDescending(x => x.CreatedAt)
                    .Select(x => new CustomerAddressDto
                    {
                        Id = x.Id,
                        CustomerId = x.CustomerId,
                        ReceiverName = x.ReceiverName,
                        PhoneNumber = x.PhoneNumber,
                        AddressLine = x.AddressLine,
                        Ward = x.Ward,
                        District = x.District,
                        City = x.City,
                        CountryCode = x.CountryCode,
                        IsDefault = x.IsDefault
                    })
                    .ToList()
            };
        }

        public async Task<PagedResultDto<CustomerListItemDto>> GetPagedAsync(
            string? keyword,
            CustomerStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 20;

            return await _customerRepository.GetPagedAsync(
                keyword,
                status,
                pageIndex,
                pageSize,
                cancellationToken);
        }

        private static CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                UserId = customer.UserId,
                Email = customer.Email,
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber,
                DateOfBirth = customer.DateOfBirth,
                AvatarUrl = customer.AvatarUrl,
                Note = customer.Note,
                Status = customer.Status,
                LastLoginAt = customer.LastLoginAt,
                CreatedAt = customer.CreatedAt
            };
        }
    }
}
