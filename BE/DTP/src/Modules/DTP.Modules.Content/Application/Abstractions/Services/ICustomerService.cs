
using DTP.Modules.Customer.Domain.Enums;
using DTP.Modules.Customer.Application.Commands.Customers;
using DTP.Modules.Customer.Application.DTOs;
using DTP.Shared.Application.Pagination;


namespace DTP.Modules.Customer.Application.Abstractions.Services
{
    public interface ICustomerService
    {
        Task<Guid> CreateAsync(
            CreateCustomerCommand command,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            UpdateCustomerCommand command,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            DeleteCustomerCommand command,
            CancellationToken cancellationToken = default);

        Task<bool> ChangeStatusAsync(
            ChangeCustomerStatusCommand command,
            CancellationToken cancellationToken = default);

        Task<CustomerDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<CustomerDetailDto?> GetDetailAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<CustomerDto?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<CustomerListItemDto>> GetPagedAsync(
            string? keyword,
            CustomerStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
