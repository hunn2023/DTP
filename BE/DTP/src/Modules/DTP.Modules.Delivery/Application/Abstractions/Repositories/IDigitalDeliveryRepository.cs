using DTP.Modules.Delivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Abstractions.Repositories
{
    public interface IDigitalDeliveryRepository
    {
        Task<List<DigitalDelivery>> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            DigitalDelivery delivery,
            CancellationToken cancellationToken = default);

        Task AddLogAsync(
            DeliveryLog log,
            CancellationToken cancellationToken = default);

        IQueryable<DigitalDelivery> Query();
    }
}
