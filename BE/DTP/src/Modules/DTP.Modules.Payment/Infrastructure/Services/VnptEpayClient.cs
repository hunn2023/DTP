using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Payment.Infrastructure.Clients;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public class VnptEpayClient : IVnptEpayClient
    {
        private readonly HttpClient _httpClient;
        private readonly VnptEpayOptions _options;

        public VnptEpayClient(
            HttpClient httpClient,
            IOptions<VnptEpayOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<VnptEpayCreateQrResponse> CreateQrAsync(
            VnptEpayCreateQrRequest request,
            CancellationToken cancellationToken = default)
        {
            request.MerchantCode = _options.MerchantCode;
            request.ReturnUrl = _options.ReturnUrl;
            request.CallbackUrl = _options.CallbackUrl;
            request.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            request.Signature = SignCreateQr(request);

            var rawRequest = JsonSerializer.Serialize(request);

            using var content = new StringContent(
                rawRequest,
                Encoding.UTF8,
                "application/json");

            var httpResponse = await _httpClient.PostAsync(
                _options.CreateQrEndpoint,
                content,
                cancellationToken);

            var rawResponse = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return new VnptEpayCreateQrResponse
                {
                    IsSuccess = false,
                    ResponseCode = ((int)httpResponse.StatusCode).ToString(),
                    Message = "VNPT ePay create QR failed.",
                    RawRequest = rawRequest,
                    RawResponse = rawResponse
                };
            }

            /*
             * TODO:
             * Mapping response theo tài liệu VNPT ePay thật.
             * Phần dưới đang là mapping mẫu.
             */
            using var document = JsonDocument.Parse(rawResponse);
            var root = document.RootElement;

            var responseCode = GetString(root, "responseCode");
            var message = GetString(root, "message");

            var isSuccess = responseCode == "00" || responseCode == "SUCCESS";

            return new VnptEpayCreateQrResponse
            {
                IsSuccess = isSuccess,
                ResponseCode = responseCode,
                Message = message,
                RequestId = GetString(root, "requestId"),
                ProviderTransactionId = GetString(root, "transactionId"),
                ProviderPaymentCode = GetString(root, "paymentCode"),
                QrCode = GetString(root, "qrCode"),
                QrImageUrl = GetString(root, "qrImageUrl"),
                PaymentUrl = GetString(root, "paymentUrl"),
                BankCode = GetString(root, "bankCode"),
                BankAccountNo = GetString(root, "bankAccountNo"),
                BankAccountName = GetString(root, "bankAccountName"),
                TransferContent = GetString(root, "transferContent"),
                ExpiredAt = DateTime.UtcNow.AddMinutes(_options.QrExpiredMinutes),
                RawRequest = rawRequest,
                RawResponse = rawResponse
            };
        }

        public bool VerifyCallbackSignature(VnptEpayCallbackDto callback)
        {
            if (string.IsNullOrWhiteSpace(callback.Signature))
                return false;

            var raw = string.Join("|", new[]
            {
            callback.MerchantCode,
            callback.RequestId,
            callback.OrderCode,
            callback.ProviderTransactionId,
            callback.Amount.ToString("0"),
            callback.Currency,
            callback.Status,
            callback.ResponseCode,
            callback.Timestamp.ToString()
        });

            var expected = HmacSha256(raw, _options.SecretKey);

            return FixedTimeEquals(expected, callback.Signature);
        }

        private string SignCreateQr(VnptEpayCreateQrRequest request)
        {
            /*
             * TODO:
             * Thứ tự field ký cần đổi đúng theo tài liệu VNPT ePay.
             */
            var raw = string.Join("|", new[]
            {
            request.MerchantCode,
            request.RequestId,
            request.OrderCode,
            request.Amount.ToString("0"),
            request.Currency,
            request.Description,
            request.ReturnUrl,
            request.CallbackUrl,
            request.Timestamp.ToString()
        });

            return HmacSha256(raw, _options.SecretKey);
        }

        private static string HmacSha256(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);

            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        private static bool FixedTimeEquals(string left, string right)
        {
            var leftBytes = Encoding.UTF8.GetBytes(left);
            var rightBytes = Encoding.UTF8.GetBytes(right);

            return leftBytes.Length == rightBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
        }

        private static string? GetString(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property))
                return null;

            return property.ValueKind switch
            {
                JsonValueKind.String => property.GetString(),
                JsonValueKind.Number => property.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => property.GetRawText()
            };
        }
    }
}
