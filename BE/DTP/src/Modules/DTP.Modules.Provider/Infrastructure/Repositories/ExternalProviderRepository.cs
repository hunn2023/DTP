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
    public class ExternalProviderRepository : IExternalProviderRepository
    {
        private readonly ProviderDbContext _context;

        public ExternalProviderRepository(ProviderDbContext context)
        {
            _context = context;
        }

        public async Task<ExternalProvider?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.ExternalProviders
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<ExternalProvider?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            return await _context.ExternalProviders
                .FirstOrDefaultAsync(x => x.Code == code && !x.IsDeleted, cancellationToken);
        }

        public async Task<List<ExternalProvider>> GetActiveAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.ExternalProviders
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsCodeAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            return await _context.ExternalProviders
                .AnyAsync(x =>
                    !x.IsDeleted &&
                    x.Code == code &&
                    (!excludeId.HasValue || x.Id != excludeId.Value),
                    cancellationToken);
        }

        public async Task AddAsync(
            ExternalProvider provider,
            CancellationToken cancellationToken = default)
        {
            await _context.ExternalProviders.AddAsync(provider, cancellationToken);
        }

        public void Update(ExternalProvider provider)
        {
            _context.ExternalProviders.Update(provider);
        }

        public void Remove(ExternalProvider provider)
        {
            provider.Delete();
            _context.ExternalProviders.Update(provider);
        }
    }
}
