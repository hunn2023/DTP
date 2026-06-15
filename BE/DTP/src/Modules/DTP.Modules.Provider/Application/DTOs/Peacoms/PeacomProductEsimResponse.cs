using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs.Peacoms
{
    public class PeacomProductEsimResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("sku")]
        public string Sku { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("regional")]
        public string? Regional { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; } = "VND";

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("periodNum")]
        public int? PeriodNum { get; set; }

        [JsonPropertyName("validity")]
        public int Validity { get; set; }

        [JsonPropertyName("dataType")]
        public int DataType { get; set; }

        [JsonPropertyName("extraData")]
        public PeacomProductEsimExtraDataResponse? ExtraData { get; set; }
    }


    public class PeacomProductEsimExtraDataResponse
    {
        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("image")]
        public JsonElement? Image { get; set; }

        [JsonPropertyName("speed")]
        public string? Speed { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("volume")]
        public JsonElement? Volume { get; set; }

        [JsonPropertyName("dataType")]
        public int? DataType { get; set; }

        [JsonPropertyName("duration")]
        public int? Duration { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("operator")]
        public PeacomProductEsimOperatorResponse? Operator { get; set; }

        [JsonPropertyName("smsStatus")]
        public int? SmsStatus { get; set; }

        [JsonPropertyName("activeType")]
        public int? ActiveType { get; set; }

        [JsonPropertyName("unusedValidTime")]
        public int? UnusedValidTime { get; set; }

        [JsonPropertyName("supportTopUpType")]
        public int? SupportTopUpType { get; set; }
    }

    public class PeacomProductEsimOperatorResponse
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("usageRange")]
        public string? UsageRange { get; set; }

        [JsonPropertyName("remark")]
        public string? Remark { get; set; }

        [JsonPropertyName("info")]
        public List<string>? Info { get; set; }

        [JsonPropertyName("esim_type")]
        public string? EsimType { get; set; }

        [JsonPropertyName("plan_type")]
        public string? PlanType { get; set; }

        [JsonPropertyName("other_info")]
        public string? OtherInfo { get; set; }

        [JsonPropertyName("is_kyc_verify")]
        public bool? IsKycVerify { get; set; }

        [JsonPropertyName("rechargeability")]
        public bool? Rechargeability { get; set; }

        [JsonPropertyName("activation_policy")]
        public string? ActivationPolicy { get; set; }

        [JsonPropertyName("coverages")]
        public List<PeacomCoverageResponse> Coverages { get; set; } = new();
    }

    public class PeacomCoverageResponse
    {
        [JsonPropertyName("locationCode")]
        public string? LocationCode { get; set; }

        [JsonPropertyName("locationLogo")]
        public string? LocationLogo { get; set; }

        [JsonPropertyName("locationName")]
        public string? LocationName { get; set; }

        [JsonPropertyName("operatorList")]
        public List<PeacomCoverageOperatorResponse>? OperatorList { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("networks")]
        public List<PeacomCoverageNetworkResponse>? Networks { get; set; }
    }

    public class PeacomCoverageOperatorResponse
    {
        [JsonPropertyName("operatorName")]
        public string? OperatorName { get; set; }

        [JsonPropertyName("networkType")]
        public string? NetworkType { get; set; }
    }

    public class PeacomCoverageNetworkResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("types")]
        public List<string>? Types { get; set; }
    }
}
