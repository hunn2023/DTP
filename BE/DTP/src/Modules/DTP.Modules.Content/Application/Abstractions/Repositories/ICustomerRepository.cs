
using DTP.Modules.Customer.Application.DTOs;
using DTP.Modules.Customer.Domain.Enums;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Persistence;

namespace DTP.Modules.Customer.Application.Abstractions.Repositories
{
    public interface ICustomerRepository : IRepositoryBase<Domain.Entities.Customer>
    {

        Task<Domain.Entities.Customer?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<Domain.Entities.Customer?> GetDetailByIdAsync(
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
    }
}
