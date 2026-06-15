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

        public async Task<PeacomCreateOrderResponse> CreateOrderAsync(
            PeacomCreateOrderRequest request,
            CancellationToken cancellationToken = default)
        {
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
    }
}
