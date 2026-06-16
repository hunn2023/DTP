using Azure.Core;
using DTP.Modules.Provider.Application.Abstractions.Clients;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Modules.Provider.Application.DTOs.Peacoms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Clients
{
    public class PeacomProviderClient : IPeacomProviderClient
    {
        private readonly HttpClient _httpClient;

        public PeacomProviderClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

       

        public async Task<IReadOnlyList<ProviderPackageProductRemoteDto>> GetPackageProductsAsync(
                Domain.Entities.Provider provider,
                CancellationToken cancellationToken = default)
        {
            EnsureHttpClientConfigured();

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/eip/partner/v2/product?page=1&size=100&type=1");

            request.Headers.Add("apikey", provider.ApiKey);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Peacom GET PACKAGE PRODUCT thất bại. StatusCode={(int)response.StatusCode}, Body={rawJson}");
            }

            var result = JsonSerializer.Deserialize<PeacomPackageProductResponse>(
                rawJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result?.Data == null || result.Data.Count == 0)
                return Array.Empty<ProviderPackageProductRemoteDto>();

            return result.Data.Select(x => new ProviderPackageProductRemoteDto
            {
                Id = x.Id.ToString(),
                Sku = x.Sku,
                Name = x.Name,
                Price = x.Price,
                Description = x.Description,
                AvailableQty = x.AvailableQty,
                Type = x.Type,
                ImageUrl = x.ImageUrl,
                RawJson = JsonSerializer.Serialize(x)
            }).ToList();
        }

        private void EnsureHttpClientConfigured()
        {
            if (_httpClient.BaseAddress == null)
            {
                throw new InvalidOperationException(
                    "PeacomProviderClient BaseAddress đang NULL. Kiểm tra AddHttpClient<IPeacomProviderClient, PeacomProviderClient>.");
            }
        }

        public async Task<ProviderEsimProductRemoteDto> GetProductEsimAsync(
                Domain.Entities.Provider provider,
                string sku,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU không được để trống.", nameof(sku));

            if (string.IsNullOrWhiteSpace(provider.ApiKey))
                throw new InvalidOperationException("Provider chưa cấu hình ApiKey.");

            
            var request = new HttpRequestMessage(
                  HttpMethod.Get,
                  $"/eip/partner/v2/product/esim?sku={Uri.EscapeDataString(sku)}");

         
            request.Headers.Add("apikey", provider.ApiKey);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Peacom GET PRODUCT ESIM thất bại. SKU={sku}, StatusCode={(int)response.StatusCode}, Body={rawJson}");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var listResult = JsonSerializer.Deserialize<PeacomProductEsimListResponse>(
                rawJson,
                options);

            var result = listResult?.Items?
                .FirstOrDefault(x => string.Equals(x.Sku, sku, StringComparison.OrdinalIgnoreCase))
                ?? listResult?.Items?.FirstOrDefault();

            if (result == null)
            {
                throw new InvalidOperationException(
                    $"Peacom PRODUCT ESIM không có dữ liệu. SKU={sku}, Body={rawJson}");
            }

            var extraData = result.ExtraData;
            var operatorInfo = extraData?.Operator;

            var dataAmount = ParseVolume(extraData?.Volume);
            var isUnlimited = IsUnlimitedVolume(extraData?.Volume);

            var countries = BuildCountries(operatorInfo?.Coverages);

            var operators = countries
                .SelectMany(x => x.Operators)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var coverageType =
                operatorInfo?.Type
                ?? (countries.Count > 1 ? "regional" : "local");

            var coverageDescription =
                operatorInfo?.UsageRange
                ?? result.Regional
                ?? extraData?.Location;

            return new ProviderEsimProductRemoteDto
            {
                Sku = result.Sku,

                Name = !string.IsNullOrWhiteSpace(extraData?.Title)
                    ? extraData.Title
                    : result.Name,

                Price = result.Price,

                CurrencyCode = string.IsNullOrWhiteSpace(result.CurrencyCode)
                    ? "VND"
                    : result.CurrencyCode,

                DataAmount = dataAmount,

                DataUnit = isUnlimited
                    ? null
                    : "MB",

                ValidityDays = extraData?.Duration ?? result.Validity,

                IsUnlimited = isUnlimited,

                CoverageType = coverageType,

                CoverageDescription = coverageDescription,

                ActivationPolicy =
                    operatorInfo?.ActivationPolicy
                    ?? MapActivationPolicy(extraData?.ActiveType),

                SpeedPolicy = extraData?.Speed,

                HotspotSupported = true,

                PhoneNumberSupported = false,

                SmsSupported = extraData?.SmsStatus == 1,

                KycRequired = operatorInfo?.IsKycVerify ?? false,

                Countries = countries,

                Operators = operators,

                RawJson = rawJson
            };
        }

        private static string? MapActivationPolicy(int? activeType)
        {
            return activeType switch
            {
                1 => "activate_on_first_use",
                2 => "activate_on_install",
                3 => "activate_after_purchase",
                _ => activeType?.ToString()
            };
        }


        public async Task<PeacomCreateOrderResponse> CreateOrderAsync(
            PeacomCreateOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            EnsureHttpClientConfigured();

            var response = await _httpClient.PostAsJsonAsync(
                "/eip/partner/v2/order",
                request,
                cancellationToken);

            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<PeacomCreateOrderResponse>(
                rawJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result is null)
                throw new InvalidOperationException("Không parse được response CREATE ORDER Peacom.");

            result.RawJson = rawJson;

            if (!result.Success)
                throw new InvalidOperationException("Peacom CREATE ORDER thất bại.");

            if (string.IsNullOrWhiteSpace(result.OrderPublicId))
                throw new InvalidOperationException("Peacom không trả orderPublicId.");

            return result;
        }

        public async Task<PeacomConfirmOrderResponse> ConfirmOrderAsync(
            string publicId,
            bool isConfirm,
            CancellationToken cancellationToken = default)
        {
            var body = new
            {
                isConfirm
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"/eip/partner/v2/order/{publicId}/confirm",
                body,
                cancellationToken);

            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<PeacomConfirmOrderResponse>(
                rawJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result is null)
                throw new InvalidOperationException("Không parse được response CONFIRM ORDER Peacom.");

            result.RawJson = rawJson;

            if (!result.Success)
                throw new InvalidOperationException("Peacom CONFIRM ORDER thất bại.");

            return result;
        }

        public async Task<PeacomRedeemResponse> RedeemAsync(
            PeacomRedeemRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/eip/partner/v2/redeem",
                request,
                cancellationToken);

            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<PeacomRedeemResponse>(
                rawJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result is null)
                throw new InvalidOperationException("Không parse được response REDEEM Peacom.");

            result.RawJson = rawJson;

            if (!result.Success)
                throw new InvalidOperationException("Peacom REDEEM thất bại.");

            return result;
        }

        public async Task<PeacomRedeemInfoResponse> GetRedeemInfoAsync(
            string serial,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(
                $"/eip/partner/v2/redeem/{serial}",
                cancellationToken);

            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<PeacomRedeemInfoResponse>(
                rawJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result is null)
                throw new InvalidOperationException("Không parse được response GET REDEEM INFO Peacom.");

            result.RawJson = rawJson;

            return result;
        }


        private static decimal? ParseVolume(JsonElement? volume)
        {
            if (volume == null)
                return null;

            var element = volume.Value;

            if (element.ValueKind == JsonValueKind.Number)
            {
                if (element.TryGetDecimal(out var number))
                    return number;
            }

            if (element.ValueKind == JsonValueKind.String)
            {
                var text = element.GetString();

                if (string.IsNullOrWhiteSpace(text))
                    return null;

                if (text.Equals("unlimited", StringComparison.OrdinalIgnoreCase))
                    return null;

                if (decimal.TryParse(
                        text,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var number))
                {
                    return number;
                }
            }

            return null;
        }

        private static bool IsUnlimitedVolume(JsonElement? volume)
        {
            if (volume == null)
                return false;

            var element = volume.Value;

            if (element.ValueKind != JsonValueKind.String)
                return false;

            var text = element.GetString();

            return text != null &&
                   text.Equals("unlimited", StringComparison.OrdinalIgnoreCase);
        }

        private static List<ProviderCoverageCountryDto> BuildCountries(
                List<PeacomCoverageResponse>? coverages)
        {
            if (coverages == null || coverages.Count == 0)
                return new List<ProviderCoverageCountryDto>();

            var countries = new List<ProviderCoverageCountryDto>();

            foreach (var coverage in coverages)
            {
                var code = coverage.LocationCode ?? coverage.Code;
                var name = coverage.LocationName ?? coverage.Name ?? code;
                var logoUrl = coverage.LocationLogo;

                if (string.IsNullOrWhiteSpace(code))
                    continue;

                var operators = new List<string>();
                var networkTypes = new List<string>();

                if (coverage.OperatorList != null)
                {
                    foreach (var op in coverage.OperatorList)
                    {
                        if (!string.IsNullOrWhiteSpace(op.OperatorName))
                            operators.Add(op.OperatorName);

                        if (!string.IsNullOrWhiteSpace(op.NetworkType))
                            networkTypes.Add(op.NetworkType);
                    }
                }

                if (coverage.Networks != null)
                {
                    foreach (var network in coverage.Networks)
                    {
                        if (!string.IsNullOrWhiteSpace(network.Name))
                            operators.Add(network.Name);

                        if (network.Types != null)
                            networkTypes.AddRange(network.Types.Where(x => !string.IsNullOrWhiteSpace(x)));
                    }
                }

                countries.Add(new ProviderCoverageCountryDto
                {
                    Code = code.Trim().ToUpperInvariant(),
                    Name = name ?? code,
                    LogoUrl = logoUrl,
                    Operators = operators.Distinct().ToList(),
                    NetworkTypes = networkTypes.Distinct().ToList()
                });
            }

            return countries;
        }
    }
}
