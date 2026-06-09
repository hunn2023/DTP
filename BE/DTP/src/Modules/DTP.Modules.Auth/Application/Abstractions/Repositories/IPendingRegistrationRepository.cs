using DTP.Modules.Auth.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;


namespace DTP.Modules.Auth.Application.Abstractions.Repositories
{
    public interface IPendingRegistrationRepository : IRepositoryBase<PendingRegistration>
    {
        Task<PendingRegistration?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task DeleteByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);
    }
}
