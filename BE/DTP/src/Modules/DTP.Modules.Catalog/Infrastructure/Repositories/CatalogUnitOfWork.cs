using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Infrastructure.Persistence;


namespace DTP.Modules.Catalog.Infrastructure.Repositories
{
    public class CatalogUnitOfWork : ICatalogUnitOfWork
    {
        private readonly CatalogDbContext _context;

        public CatalogUnitOfWork(CatalogDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public void ClearTracking()
        {
            _context.ChangeTracker.Clear();
        }
    }
}
