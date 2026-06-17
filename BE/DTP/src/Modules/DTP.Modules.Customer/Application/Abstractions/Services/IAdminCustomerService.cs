using DTP.Modules.Customer.Application.DTOs;
using DTP.Shared.Application;

namespace DTP.Modules.Customer.Application.Abstractions.Services
{
    public interface IAdminCustomerService
    {
        Task<Result<CustomerStatusResultDto>> UpdateStatusAsync(
            Guid userId,
            bool isActive,
            string? reason,
            Guid updatedByUserId,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default);
    }
}
