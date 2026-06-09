using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure
{
    public class ProviderUnitOfWork : IProviderUnitOfWork
    {
        private readonly ProviderDbContext _context;

        public ProviderUnitOfWork(ProviderDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
