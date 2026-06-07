using DTP.Modules.Auth.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;


namespace DTP.Modules.Auth.Application.Abstractions.Repositories
{

    public interface IAuthAttemptLogRepository : IRepositoryBase<AuthAttemptLog>
    {
        Task<int> CountFailedAsync(
            string actionType,
            string identifier,
            DateTime fromTime,
            CancellationToken cancellationToken = default);
    }
}
