using DTP.Modules.Provider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProviderOrderDetailDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public Guid ProviderId { get; set; }

        public string ProviderName { get; set; } = default!;

        public string? ProviderOrderCode { get; set; }

        public ProviderOrderStatus Status { get; set; }

        public int RetryCount { get; set; }

        public string? ErrorCode { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public List<ProviderOrderItemDto> Items { get; set; } = new();
    }

    public class ProviderOrderItemDto
    {
        public Guid Id { get; set; }

        public Guid OrderItemId { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductVariantId { get; set; }

        public string ProviderProductCode { get; set; } = default!;

        public int Quantity { get; set; }

        public string? Iccid { get; set; }

        public string? Msisdn { get; set; }

        public string? QrCodeUrl { get; set; }

        public string? QrCodeText { get; set; }

        public string? ActivationCode { get; set; }

        public string? Serial { get; set; }

        public string? PinCode { get; set; }

        public DateTime? ExpiredAt { get; set; }
    }
}
