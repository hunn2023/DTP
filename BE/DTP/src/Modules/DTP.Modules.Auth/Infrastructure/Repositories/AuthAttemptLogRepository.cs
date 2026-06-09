using DTP.Modules.Auth.Application.Abstractions.Repositories;
using DTP.Modules.Auth.Domain.Entities;
using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Auth.Infrastructure.Repositories
{
    public class AuthAttemptLogRepository
        : RepositoryBase<AuthAttemptLog>,
          IAuthAttemptLogRepository
    {
        private readonly AuthDbContext _context;

        public AuthAttemptLogRepository(AuthDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<int> CountFailedAsync(
            string actionType,
            string identifier,
            DateTime fromTime,
            CancellationToken cancellationToken = default)
        {
            return await _context.AuthAttemptLogs
                .AsNoTracking()
                .CountAsync(x =>
                    x.ActionType == actionType &&
                    x.Identifier == identifier &&
                    !x.Success &&
                    x.CreatedAt >= fromTime,
                    cancellationToken);
        }
    }
}
