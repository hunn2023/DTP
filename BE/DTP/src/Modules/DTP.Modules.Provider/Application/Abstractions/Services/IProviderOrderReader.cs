using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Services
{
    public interface IProviderOrderReader
    {
        Task<ProviderDtpOrderDto?> GetOrderForProviderAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }

    public class ProviderDtpOrderDto
    {
        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public string CustomerEmail { get; set; } = default!;

        public string CustomerName { get; set; } = default!;

        public string CustomerPhone { get; set; } = default!;

        public List<ProviderDtpOrderItemDto> Items { get; set; } = new();
    }

    public class ProviderDtpOrderItemDto
    {
        public Guid OrderItemId { get; set; }

        public Guid EsimPackageId { get; set; }

        public string Sku { get; set; } = default!;

        public int Quantity { get; set; }
    }
}
