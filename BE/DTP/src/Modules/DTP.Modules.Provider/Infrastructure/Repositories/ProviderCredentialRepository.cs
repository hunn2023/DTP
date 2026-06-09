using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Modules.Provider.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Repositories
{
    public class ProviderCredentialRepository : IProviderCredentialRepository
    {
        private readonly ProviderDbContext _context;

        public ProviderCredentialRepository(ProviderDbContext context)
        {
            _context = context;
        }

        public async Task<ProviderCredential?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProviderCredentials
                .Include(x => x.Provider)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<List<ProviderCredential>> GetByProviderIdAsync(
            Guid providerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProviderCredentials
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ProviderId == providerId)
                .OrderBy(x => x.Key)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProviderCredential?> GetActiveByKeyAsync(
            Guid providerId,
            string key,
            CancellationToken cancellationToken = default)
        {
            key = key.Trim();

            return await _context.ProviderCredentials
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    x.ProviderId == providerId &&
                    x.Key == key,
                    cancellationToken);
        }

        public async Task<bool> ExistsKeyAsync(
            Guid providerId,
            string key,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            key = key.Trim();

            return await _context.ProviderCredentials
                .AnyAsync(x =>
                    !x.IsDeleted &&
                    x.ProviderId == providerId &&
                    x.Key == key &&
                    (!excludeId.HasValue || x.Id != excludeId.Value),
                    cancellationToken);
        }

        public async Task AddAsync(
            ProviderCredential credential,
            CancellationToken cancellationToken = default)
        {
            await _context.ProviderCredentials.AddAsync(credential, cancellationToken);
        }

        public void Update(ProviderCredential credential)
        {
            _context.ProviderCredentials.Update(credential);
        }

        public void Remove(ProviderCredential credential)
        {
            credential.Delete();
            _context.ProviderCredentials.Update(credential);
        }
    }
}
