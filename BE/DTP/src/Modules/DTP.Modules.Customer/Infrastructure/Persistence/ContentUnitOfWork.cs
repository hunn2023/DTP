using DTP.Modules.Content.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Infrastructure.Persistence
{
    public class ContentUnitOfWork : IContentUnitOfWork
    {
        private readonly ContentDbContext _dbContext;

        public ContentUnitOfWork(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
