using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.DTOs
{
    public class ImportEsimProfileDto
    {
        public Guid ProductId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public Guid? EsimPackageId { get; set; }

        public Guid? ProviderId { get; set; }

        public string Iccid { get; set; } = default!;

        public string? Imsi { get; set; }

        public string? Msisdn { get; set; }

        public string ActivationCode { get; set; } = default!;

        public string? QrCodeUrl { get; set; }

        public string? QrContent { get; set; }

        public string? SmdpAddress { get; set; }

        public string? MatchingId { get; set; }

        public DateTime? ExpiredAt { get; set; }
    }
}
