using DTP.Modules.Provider.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Services
{
    public interface IProviderProvisioningService
    {
        Task<ProviderProvisionResultDto> ProvisionOrderAsync(
            Guid orderId,
            string orderCode,
            List<ProviderProvisionOrderItemInput> items,
            CancellationToken cancellationToken = default);

        Task<ProviderProvisionResultDto> RetryAsync(
            Guid providerOrderId,
            CancellationToken cancellationToken = default);
    }

    public class ProviderProvisionOrderItemInput
    {
        public Guid OrderItemId { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductVariantId { get; set; }

        public int Quantity { get; set; }
    }
}
