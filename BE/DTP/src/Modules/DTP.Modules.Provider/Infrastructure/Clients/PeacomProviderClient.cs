using Azure.Core;
using DTP.Modules.Provider.Application.Abstractions.Clients;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Modules.Provider.Application.DTOs.Peacoms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            int pageSize = 100,
            CancellationToken cancellationToken = default)
        {
            EnsureHttpClientConfigured();

            if (pageSize <= 0)
                pageSize = 100;

            var pageIndex = 1;
            var products = new List<ProviderPackageProductRemoteDto>();

            while (true)
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"/eip/partner/v2/product?page={pageIndex}&size={pageSize}&type=1");

                request.Headers.Add("apikey", provider.ApiKey);

                using var response = await _httpClient.SendAsync(
                    request,
                    cancellationToken);

                var rawJson = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        $"Peacom GET PACKAGE PRODUCT thất bại. " +
                        $"Page={pageIndex}, Size={pageSize}, " +
                        $"StatusCode={(int)response.StatusCode}, Body={rawJson}");
                }

                var result = JsonSerializer.Deserialize<PeacomPackageProductResponse>(
                    rawJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                var pageData = result?.Data;

                if (pageData == null || pageData.Count == 0)
                    break;

                products.AddRange(pageData.Select(x =>
                    new ProviderPackageProductRemoteDto
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
                    }));

                // Số lượng trả về nhỏ hơn pageSize nghĩa là đã đến trang cuối.
                if (pageData.Count < pageSize)
                    break;

                pageIndex++;
            }

            return products;
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
                Slug = extraData.Slug,

                Name = !string.IsNullOrWhiteSpace(extraData?.Title)
                    ? extraData.Title
                    : result.Name,

                Price = result.Price,

                CurrencyCode = string.IsNullOrWhiteSpace(result.CurrencyCode)
                    ? "VND"
                    : result.CurrencyCode,

                DataAmount = dataAmount,
                DataType = result.DataType,
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
             Domain.Entities.Provider provider,
             PeacomCreateOrderRequest request,
             CancellationToken cancellationToken = default)
        {
            EnsureHttpClientConfigured();

            if (provider is null)
                throw new ArgumentNullException(nameof(provider));

            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(provider.ApiKey))
                throw new InvalidOperationException("Provider chưa cấu hình ApiKey.");

            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true
            };

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "eip/partner/v2/order");

            httpRequest.Headers.Add("apikey", provider.ApiKey);

            httpRequest.Content = JsonContent.Create(
                request,
                options: jsonOptions);

            using var response = await _httpClient.SendAsync(
                httpRequest,
                cancellationToken);

            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Gọi Peacom CREATE ORDER thất bại. " +
                    $"HttpStatus={(int)response.StatusCode} {response.ReasonPhrase}. " +
                    $"Response={rawJson}");
            }

            PeacomCreateOrderResponse? result;

            try
            {
                result = JsonSerializer.Deserialize<PeacomCreateOrderResponse>(
                    rawJson,
                    jsonOptions);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Không parse được response CREATE ORDER Peacom. Response={rawJson}",
                    ex);
            }

            if (result is null)
            {
                throw new InvalidOperationException(
                    $"Không parse được response CREATE ORDER Peacom. Response={rawJson}");
            }

            result.RawJson = rawJson;

            if (!result.Success && 1 == 10)
            {
                throw new InvalidOperationException(
                    $"Peacom CREATE ORDER thất bại. Response={rawJson}");
            }

            if (string.IsNullOrWhiteSpace(result.OrderPublicId))
            {
                throw new InvalidOperationException(
                    $"Peacom CREATE ORDER thành công nhưng không trả orderPublicId. Response={rawJson}");
            }

            return result;
        }

        public async Task<PeacomConfirmOrderResponse> ConfirmOrderAsync(
            Domain.Entities.Provider provider,
            string publicId,
            bool isConfirm,
            CancellationToken cancellationToken = default)
        {
            EnsureHttpClientConfigured();

            if (provider is null)
                throw new ArgumentNullException(nameof(provider));

            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("publicId không được rỗng.", nameof(publicId));

            if (string.IsNullOrWhiteSpace(provider.ApiKey))
                throw new InvalidOperationException("Provider chưa cấu hình ApiKey.");

            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true
            };

            var body = new
            {
                isConfirm
            };

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"eip/partner/v2/order/{Uri.EscapeDataString(publicId)}/confirm");

            httpRequest.Headers.Add("apikey", provider.ApiKey);

            httpRequest.Content = JsonContent.Create(
                body,
                options: jsonOptions);

            using var response = await _httpClient.SendAsync(
                httpRequest,
                cancellationToken);

            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Gọi Peacom CONFIRM ORDER thất bại. " +
                    $"PublicId={publicId}. " +
                    $"IsConfirm={isConfirm}. " +
                    $"HttpStatus={(int)response.StatusCode} {response.ReasonPhrase}. " +
                    $"Response={rawJson}");
            }

            PeacomConfirmOrderResponse? result;

            try
            {
                result = JsonSerializer.Deserialize<PeacomConfirmOrderResponse>(
                    rawJson,
                    jsonOptions);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Không parse được response CONFIRM ORDER Peacom. " +
                    $"PublicId={publicId}. Response={rawJson}",
                    ex);
            }

            if (result is null)
            {
                throw new InvalidOperationException(
                    $"Không parse được response CONFIRM ORDER Peacom. " +
                    $"PublicId={publicId}. Response={rawJson}");
            }

            result.RawJson = rawJson;

            if (!result.Success)
            {
                throw new InvalidOperationException(
                    $"Peacom CONFIRM ORDER thất bại. " +
                    $"PublicId={publicId}. IsConfirm={isConfirm}. Response={rawJson}");
            }

            return result;
        }

        public async Task<PeacomRedeemResponse> RedeemAsync(
             Domain.Entities.Provider provider,
             PeacomRedeemRequest request,
             CancellationToken cancellationToken = default)
        {
            EnsureHttpClientConfigured();

            if (provider is null)
                throw new ArgumentNullException(nameof(provider));

            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(provider.ApiKey))
                throw new InvalidOperationException("Provider chưa cấu hình ApiKey.");

            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true
            };

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "eip/partner/v2/redeem");

            httpRequest.Headers.Add("apikey", provider.ApiKey);

            httpRequest.Content = JsonContent.Create(
                request,
                options: jsonOptions);

            using var response = await _httpClient.SendAsync(
                httpRequest,
                cancellationToken);

            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Gọi Peacom REDEEM thất bại. " +
                    $"HttpStatus={(int)response.StatusCode} {response.ReasonPhrase}. " +
                    $"Response={rawJson}");
            }

            PeacomRedeemResponse? result;

            try
            {
                result = JsonSerializer.Deserialize<PeacomRedeemResponse>(
                    rawJson,
                    jsonOptions);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Không parse được response REDEEM Peacom. Response={rawJson}",
                    ex);
            }

            if (result is null)
            {
                throw new InvalidOperationException(
                    $"Không parse được response REDEEM Peacom. Response={rawJson}");
            }

            result.RawJson = rawJson;

            if (!result.Success)
            {
                throw new InvalidOperationException(
                    $"Peacom REDEEM thất bại. Response={rawJson}");
            }

            return result;
        }


        public async Task<PeacomRedeemInfoResponse> GetRedeemInfoAsync(
             Domain.Entities.Provider provider,
             string serial,
             CancellationToken cancellationToken = default)
        {
            EnsureHttpClientConfigured();

            ArgumentNullException.ThrowIfNull(provider);

            if (string.IsNullOrWhiteSpace(serial))
                throw new ArgumentException("Serial không được rỗng.", nameof(serial));

            if (string.IsNullOrWhiteSpace(provider.ApiKey))
                throw new InvalidOperationException("Provider chưa cấu hình ApiKey.");

            serial = serial.Trim();

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                $"eip/partner/v2/redeem/{Uri.EscapeDataString(serial)}");

            httpRequest.Headers.TryAddWithoutValidation(
                "apikey",
                provider.ApiKey);

            using var response = await _httpClient.SendAsync(
                httpRequest,
                cancellationToken);

            var rawResponse = await response.Content.ReadAsStringAsync(
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadGateway)
            {
                throw new InvalidOperationException(
                    $"Peacom trả về 502 Bad Gateway. " +
                    $"Serial={serial}. " +
                    $"Endpoint={httpRequest.RequestUri}. " +
                    $"Response={rawResponse}");
            }

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                throw new InvalidOperationException(
                    $"Peacom trả về 503 Service Unavailable. " +
                    $"Serial={serial}. Response={rawResponse}");
            }

            if (response.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                throw new InvalidOperationException(
                    $"Peacom trả về 504 Gateway Timeout. " +
                    $"Serial={serial}. Response={rawResponse}");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Gọi Peacom GET REDEEM INFO thất bại. " +
                    $"Serial={serial}. " +
                    $"HttpStatus={(int)response.StatusCode} {response.ReasonPhrase}. " +
                    $"Response={rawResponse}");
            }

            try
            {
                var result = JsonSerializer.Deserialize<PeacomRedeemInfoResponse>(
                    rawResponse,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    });

                if (result is null)
                {
                    throw new InvalidOperationException(
                        $"Response GET REDEEM INFO Peacom là null. " +
                        $"Serial={serial}. Response={rawResponse}");
                }

                result.RawJson = rawResponse;

                return result;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Không parse được response GET REDEEM INFO Peacom. " +
                    $"Serial={serial}. Response={rawResponse}",
                    ex);
            }
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
