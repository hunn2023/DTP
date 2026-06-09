using DTP.Modules.Delivery.Application.Abstractions.Repositories;
using DTP.Modules.Delivery.Domain.Entities;
using DTP.Modules.Delivery.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Infrastructure.Repositories
{
    public class DigitalDeliveryRepository : IDigitalDeliveryRepository
    {
        private readonly DeliveryDbContext _context;

        public DigitalDeliveryRepository(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<List<DigitalDelivery>> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DigitalDeliveries
                .Where(x => x.OrderId == orderId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(
            DigitalDelivery delivery,
            CancellationToken cancellationToken = default)
        {
            await _context.DigitalDeliveries.AddAsync(delivery, cancellationToken);
        }

        public async Task AddLogAsync(
            DeliveryLog log,
            CancellationToken cancellationToken = default)
        {
            await _context.DeliveryLogs.AddAsync(log, cancellationToken);
        }

        public IQueryable<DigitalDelivery> Query()
        {
            return _context.DigitalDeliveries.AsQueryable();
        }
    }
}
