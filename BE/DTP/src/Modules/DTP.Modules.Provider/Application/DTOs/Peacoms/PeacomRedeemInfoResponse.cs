using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs.Peacoms
{
    public class PeacomRedeemInfoResponse
    {
        public string Serial { get; set; } = default!;
        public int Status { get; set; }
        public int ProductType { get; set; }

        public string Sku { get; set; } = default!;
        public string? PackageName { get; set; }
        public string? Model { get; set; }
        public DateTime? LastRequest { get; set; }

        public List<PeacomRedeemInfoDataDto> Data { get; set; } = new();

        public string RawJson { get; set; } = default!;
    }

    public class PeacomRedeemInfoDataDto
    {
        public int? ProductType { get; set; }

        // Insurance
        public string? PolicyNumber { get; set; }
        public string? RequestId { get; set; }
        public string? UrlResp { get; set; }
        public string? CardId { get; set; }
        public string? PolicyGCN { get; set; }

        // eSIM
        public string? Slug { get; set; }
        public int Volume { get; set; }
        public int? Duration { get; set; }
        public string? DurationUnit { get; set; }
        public string? Location { get; set; }
        public int? ActiveType { get; set; }
        public int? SupportTopUpType { get; set; }
        public string? Imsi { get; set; }
        public string? Iccid { get; set; }
        public string? Ac { get; set; }
        public string? QrCodeUrl { get; set; }
        public string? ShortUrlApple { get; set; }
        public string? ShortUrlAndroid { get; set; }
        public string? Apn { get; set; }
    }
}
