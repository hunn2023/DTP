using DTP.Modules.Provider.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Gateways
{
    public interface IProviderGateway
    {
        Task<ProviderProvisionResultDto> CreateOrderAsync(
            ProviderCreateOrderRequest request,
            CancellationToken cancellationToken = default);
    }

    public class ProviderCreateOrderRequest
    {
        public Guid ProviderId { get; set; }

        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public List<ProviderCreateOrderItemRequest> Items { get; set; } = new();
    }

    public class ProviderCreateOrderItemRequest
    {
        public Guid OrderItemId { get; set; }

        public string ProviderProductCode { get; set; } = default!;

        public int Quantity { get; set; }
    }
}
