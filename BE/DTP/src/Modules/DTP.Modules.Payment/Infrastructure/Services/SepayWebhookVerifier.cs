using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public sealed class SepayWebhookVerifier : ISepayWebhookVerifier
    {
        private readonly SepayOptions _options;

        public SepayWebhookVerifier(IOptions<SepayOptions> options)
        {
            _options = options.Value;
        }

        public bool Verify(IHeaderDictionary headers, string rawBody)
        {
            if (!_options.RequireWebhookSignature)
                return true;

            if (string.IsNullOrWhiteSpace(_options.WebhookSecret))
                return false;

            var signature = headers["X-SePay-Signature"].FirstOrDefault();
            var timestampText = headers["X-SePay-Timestamp"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(signature) ||
                string.IsNullOrWhiteSpace(timestampText))
            {
                return false;
            }

            if (!long.TryParse(timestampText, out var timestamp))
                return false;

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Chống replay: lệch quá 5 phút thì reject.
            if (Math.Abs(now - timestamp) > 300)
                return false;

            var payloadToSign = $"{timestampText}.{rawBody}";

            using var hmac = new HMACSHA256(
                Encoding.UTF8.GetBytes(_options.WebhookSecret));

            var hash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(payloadToSign));

            var expectedSignature =
                "sha256=" + Convert.ToHexString(hash).ToLowerInvariant();

            var expectedBytes = Encoding.UTF8.GetBytes(expectedSignature);
            var actualBytes = Encoding.UTF8.GetBytes(signature);

            return expectedBytes.Length == actualBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);
        }
    }
}
