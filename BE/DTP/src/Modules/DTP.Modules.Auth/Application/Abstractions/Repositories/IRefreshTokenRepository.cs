using DTP.Modules.Auth.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;

namespace DTP.Modules.Auth.Application.Abstractions.Repositories
{
    public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken = default);

        Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
