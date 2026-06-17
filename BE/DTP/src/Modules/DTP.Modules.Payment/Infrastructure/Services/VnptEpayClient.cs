using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Payment.Infrastructure.Clients;
using DTP.Modules.Provider.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public class VnptEpayClient : IVnptEpayClient
    {
        private readonly HttpClient _httpClient;
        private readonly VnptEpayOptions _options;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = null,
            PropertyNameCaseInsensitive = true
        };

        public VnptEpayClient(
            HttpClient httpClient,
            IOptions<VnptEpayOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<VnptEpayRegisterVaResponse> RegisterVaAsync(
            VnptEpayRegisterVaRequest request,
            CancellationToken cancellationToken = default)
        {
            ValidateRegisterVaRequest(request);

            var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7));

            var customerNameNoSign = ToUpperNoSign(request.CustomerName);
            var mapId = GenerateMapId(now);
            var requestId = GenerateRequestId(now);

            var startDate = $"{now:yyyyMMdd}000000";
            var endDate = $"{now.AddDays(_options.VaExpireDays):yyyyMMdd}235959";

            var extend = new
            {
                phone = request.Phone,
                email = request.Email,
                address = request.Address,
                id = request.ReferenceId,
                contentQR = request.ContentQr
            };

            var dataObj = new
            {
                map_id = mapId,
                amount = request.Amount,
                start_date = startDate,
                end_date = endDate,
                condition = _options.Condition,
                customer_name = customerNameNoSign,
                request_id = requestId,
                bank_code = _options.BankCode,
                extend
            };

            var dataJson = JsonSerializer.Serialize(dataObj, JsonOptions);

            // Giữ nguyên TripDESUtil hiện tại của bạn.
            var encryptedData = TripDESUtil.Encrypt(dataJson, _options.EncryptionKey);

            var epayRequest = new
            {
                pcode = _options.PCodeRegister,
                merchant_code = _options.MerchantCode,
                data = encryptedData
            };

            var json = JsonSerializer.Serialize(epayRequest, JsonOptions);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                _options.RegisterVaUrl);

            httpRequest.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            httpRequest.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            using var httpResponse = await _httpClient.SendAsync(
                httpRequest,
                cancellationToken);

            var rawResponse = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return new VnptEpayRegisterVaResponse
                {
                    ResponseCode = "-1",
                    Message = $"Lỗi HTTP khi gọi VNPT ePay. StatusCode: {(int)httpResponse.StatusCode}. Response: {rawResponse}"
                };
            }

            try
            {
                var response = JsonSerializer.Deserialize<VnptEpayRegisterVaResponse>(
                    rawResponse,
                    JsonOptions);

                if (response == null)
                {
                    return new VnptEpayRegisterVaResponse
                    {
                        ResponseCode = "-1",
                        Message = "Không parse được phản hồi từ VNPT ePay"
                    };
                }

                return response;
            }
            catch
            {
                return new VnptEpayRegisterVaResponse
                {
                    ResponseCode = "-1",
                    Message = "Lỗi phản hồi từ cổng thanh toán VNPT ePay"
                };
            }
        }

        public bool VerifyDepositNotificationSignature(Application.DTOs.VnptEpayCallbackDto callback)
        {
            var rawString = string.Join("|",
                callback.RequestId,
                callback.ReferenceId,
                callback.RequestTime,
                callback.Amount.ToString("0.##", CultureInfo.InvariantCulture),
                callback.Fee.ToString("0.##", CultureInfo.InvariantCulture),
                callback.VaAcc,
                callback.MapId);

            return Verify(rawString, callback.Signature!, _options.EpayPublicKeyPem ?? "");
        }


        public bool Verify(
            string rawString,
            string signatureHex,
            string epayPublicKeyPem)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rawString))
                    return false;

                if (string.IsNullOrWhiteSpace(signatureHex))
                    return false;

                if (string.IsNullOrWhiteSpace(epayPublicKeyPem))
                    return false;

                var dataBytes = Encoding.UTF8.GetBytes(rawString);
                var signatureBytes = HexStringToBytes(signatureHex);

                using var rsa = RSA.Create();
                rsa.ImportFromPem(epayPublicKeyPem);

                return rsa.VerifyData(
                    dataBytes,
                    signatureBytes,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);
            }
            catch
            {
                return false;
            }
        }

        private void ValidateRegisterVaRequest(VnptEpayRegisterVaRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Amount <= 0)
                throw new ArgumentException("Amount phải lớn hơn 0", nameof(request.Amount));

            if (string.IsNullOrWhiteSpace(request.CustomerName))
                throw new ArgumentException("CustomerName không được để trống", nameof(request.CustomerName));

            if (string.IsNullOrWhiteSpace(request.ReferenceId))
                throw new ArgumentException("ReferenceId không được để trống", nameof(request.ReferenceId));

            if (string.IsNullOrWhiteSpace(request.ContentQr))
                throw new ArgumentException("ContentQr không được để trống", nameof(request.ContentQr));

            if (string.IsNullOrWhiteSpace(_options.PCodeRegister))
                throw new InvalidOperationException("VnptEpay:PCodeRegister chưa được cấu hình");

            if (string.IsNullOrWhiteSpace(_options.MerchantCode))
                throw new InvalidOperationException("VnptEpay:MerchantCode chưa được cấu hình");

            if (string.IsNullOrWhiteSpace(_options.EncryptionKey))
                throw new InvalidOperationException("VnptEpay:EncryptionKey chưa được cấu hình");

            if (string.IsNullOrWhiteSpace(_options.RegisterVaUrl))
                throw new InvalidOperationException("VnptEpay:RegisterVaUrl chưa được cấu hình");

            if (string.IsNullOrWhiteSpace(_options.BankCode))
                throw new InvalidOperationException("VnptEpay:BankCode chưa được cấu hình");
        }

        private string GenerateRequestId(DateTimeOffset now)
        {
            return $"{_options.MerchantCode}{now:ddMMyyyyHHmmss}_{GenerateRandomString(5)}";
        }

        private string GenerateMapId(DateTimeOffset now)
        {
            var number = RandomNumberGenerator.GetInt32(100, 999);
            return $"{_options.MerchantCode}-{now:yyyyMMdd}_{number}";
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyz";

            var result = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                var index = RandomNumberGenerator.GetInt32(chars.Length);
                result.Append(chars[index]);
            }

            return result.ToString();
        }

        private static string ToUpperNoSign(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            var noSign = sb
                .ToString()
                .Normalize(NormalizationForm.FormC)
                .Replace('Đ', 'D')
                .Replace('đ', 'd');

            return noSign.ToUpperInvariant();
        }

        private static byte[] HexStringToBytes(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return Array.Empty<byte>();

            if (hex.Length % 2 != 0)
                throw new ArgumentException("Signature hex không hợp lệ", nameof(hex));

            var data = new byte[hex.Length / 2];

            for (var i = 0; i < hex.Length; i += 2)
            {
                data[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return data;
        }
    }
}
