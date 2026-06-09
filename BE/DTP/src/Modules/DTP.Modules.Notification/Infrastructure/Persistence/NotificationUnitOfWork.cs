using DTP.Modules.Notification.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Infrastructure.Persistence
{
    public class NotificationUnitOfWork : INotificationUnitOfWork
    {
        private readonly NotificationDbContext _context;

        public NotificationUnitOfWork(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
