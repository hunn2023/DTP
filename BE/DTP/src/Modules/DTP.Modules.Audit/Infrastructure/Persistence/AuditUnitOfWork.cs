using DTP.Modules.Audit.Application.Abstractions.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Infrastructure.Persistence
{
    public class AuditUnitOfWork : IAuditUnitOfWork
    {
        private readonly AuditDbContext _context;

        public AuditUnitOfWork(AuditDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
