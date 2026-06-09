using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Delivery
{
    public interface IOrderDeliveryReader
    {
        Task<DeliveryOrderInfoDto?> GetOrderForDeliveryAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}
