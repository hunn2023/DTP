using DTP.Modules.Customer.Application.Common;
using DTP.Modules.Customer.Application.DTOs;

namespace DTP.Modules.Customer.Application.Abstractions.Repositories
{
    public interface IAdminCustomerRepository
    {
        Task<PagedResult<AdminCustomerListItemDto>> GetPagedAsync(
            string? keyword,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<AdminCustomerDetailDto?> GetDetailAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
