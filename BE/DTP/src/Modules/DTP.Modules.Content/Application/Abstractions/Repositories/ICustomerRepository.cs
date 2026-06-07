using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.Abstractions.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Customer?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<Customer?> GetDetailByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByEmailAsync(
            string email,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByUserIdAsync(
            Guid userId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<CustomerListItemDto>> GetPagedAsync(
            string? keyword,
            CustomerStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Customer customer,
            CancellationToken cancellationToken = default);

        void Update(Customer customer);

        void Remove(Customer customer);
    }
}
