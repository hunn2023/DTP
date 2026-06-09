using DTP.Modules.Delivery.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.DTOs
{
    public class EsimProfileDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public Guid? EsimPackageId { get; set; }

        public Guid? ProviderId { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? OrderItemId { get; set; }

        public string Iccid { get; set; } = default!;

        public string? Imsi { get; set; }

        public string? Msisdn { get; set; }

        public string ActivationCode { get; set; } = default!;

        public string? QrCodeUrl { get; set; }

        public string? QrContent { get; set; }

        public string? SmdpAddress { get; set; }

        public string? MatchingId { get; set; }

        public EsimProfileStatus Status { get; set; }

        public string StatusName { get; set; } = default!;

        public DateTime CreatedAt { get; set; }

        public DateTime? AssignedAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public DateTime? ExpiredAt { get; set; }
    }
}
