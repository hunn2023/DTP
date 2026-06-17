using DTP.Modules.Chatbot.Application.Abstractions;
using DTP.Modules.Chatbot.Application.DTOs;
using DTP.Modules.Chatbot.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Infrastructure.Clients
{
    public sealed class OpenAiChatbotClient : IChatbotAiClient
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAiChatbotOptions _options;
        private readonly ILogger<OpenAiChatbotClient> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public OpenAiChatbotClient(
            HttpClient httpClient,
            IOptions<OpenAiChatbotOptions> options,
            ILogger<OpenAiChatbotClient> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<ChatbotIntentDto> ExtractIntentAsync(
            string message,
            CancellationToken cancellationToken = default)
        {
            if (_options.DisableAi || string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                return LocalExtractIntent(message);
            }

            var instructions = """
Bạn là bộ phân tích nhu cầu mua eSIM cho hệ thống DTP.

Nhiệm vụ:
- Chỉ trả về JSON hợp lệ.
- Không giải thích thêm.
- Không markdown.
- Không bịa thông tin sản phẩm.
- Phân loại intentType:
  - product_advice: khách cần tư vấn/mua/chọn gói eSIM
  - faq: khách hỏi cách dùng eSIM, cài đặt, QR, roaming, hotspot
  - order_support: khách hỏi đơn hàng, thanh toán, nhận QR
  - unknown: không rõ

Schema JSON bắt buộc:
{
  "intentType": "product_advice | faq | order_support | unknown",
  "countryKeyword": "string hoặc null",
  "countryCode": "string hoặc null",
  "travelDays": number hoặc null,
  "requestedDataAmount": number hoặc null,
  "requestedDataUnit": "GB | MB | null",
  "usageLevel": "light | normal | heavy | unlimited | null",
  "budgetType": "cheapest | balanced | premium | null",
  "needsHotspot": true hoặc false hoặc null,
  "needsPhoneNumber": true hoặc false hoặc null,
  "needsSms": true hoặc false hoặc null,
  "originalQuestion": "string"
}
Quy đổi requestedDataAmount:
- Nếu khách nói 1GB, 3GB, 5GB, 10GB thì requestedDataAmount là số tương ứng.
- Nếu khách nói 500MB thì requestedDataAmount = 500, requestedDataUnit = "MB".
- Nếu khách nói không giới hạn, unlimited, dung lượng không giới hạn thì usageLevel = "unlimited", requestedDataAmount = null.
- Nếu khách không nói rõ dung lượng thì requestedDataAmount = null.

Quy đổi usageLevel:
- light: chỉ chat, bản đồ, email ít
- normal: mạng xã hội, web, bản đồ thường xuyên
- heavy: TikTok, YouTube, video, livestream, làm việc nhiều
- unlimited: khách nói muốn không giới hạn

Quy đổi budgetType:
- cheapest: khách hỏi rẻ nhất, tiết kiệm
- balanced: khách hỏi phù hợp, ổn, nên mua
- premium: khách hỏi tốt nhất, mạnh nhất, dùng nhiều
""";

            var input = $"Câu hỏi khách hàng: {message}";

            var output = await CreateResponseAsync(
                instructions,
                input,
                maxOutputTokens: 500,
                temperature: 0.1,
                cancellationToken);

            var json = CleanJson(output);

            try
            {
                var intent = JsonSerializer.Deserialize<ChatbotIntentDto>(
                    json,
                    JsonOptions);

                return intent ?? LocalExtractIntent(message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Parse chatbot intent failed. Raw output: {Output}",
                    output);

                return LocalExtractIntent(message);
            }
        }

        public async Task<string> GenerateAnswerAsync(
            string userMessage,
            ChatbotIntentDto intent,
            IReadOnlyList<ChatbotProductSuggestionDto> suggestions,
            CancellationToken cancellationToken = default)
        {
            if (_options.DisableAi || string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                return LocalGenerateAnswer(intent, suggestions);
            }

            var instructions = """
Bạn là trợ lý tư vấn eSIM cho hệ thống DTP.

Nguyên tắc bắt buộc:
- Chỉ tư vấn sản phẩm dựa trên danh sách sản phẩm hệ thống cung cấp.
- Không tự bịa giá, dung lượng, số ngày, nhà mạng hoặc chính sách.
- Nếu không có sản phẩm phù hợp, nói rõ chưa tìm thấy gói phù hợp và hỏi thêm thông tin.
- Trả lời bằng tiếng Việt, thân thiện, ngắn gọn.
- Nếu có sản phẩm, đề xuất tối đa 3 gói.
- Luôn giải thích vì sao gói đó phù hợp.
- Không nói những gì nằm ngoài dữ liệu được cung cấp.
- Không tự tạo link khác ngoài buyUrl có sẵn.
""";

            var payload = new
            {
                userMessage,
                intent,
                suggestions
            };

            var input = JsonSerializer.Serialize(payload, JsonOptions);

            return await CreateResponseAsync(
                instructions,
                input,
                maxOutputTokens: _options.MaxOutputTokens,
                temperature: 0.3,
                cancellationToken);
        }

        private async Task<string> CreateResponseAsync(
            string instructions,
            string input,
            int maxOutputTokens,
            double temperature,
            CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/v1/responses");

            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _options.ApiKey);

            var body = new
            {
                model = _options.Model,
                instructions,
                input,
                max_output_tokens = maxOutputTokens,
                temperature,
                store = false
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(
                request,
                cancellationToken);

            var responseBody = await response.Content.ReadAsStringAsync(
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "OpenAI chatbot request failed. StatusCode: {StatusCode}. Body: {Body}",
                    response.StatusCode,
                    responseBody);

                throw new InvalidOperationException(
                    $"OpenAI chatbot request failed: {response.StatusCode}");
            }

            return ExtractOutputText(responseBody);
        }

        private static string ExtractOutputText(string responseBody)
        {
            using var document = JsonDocument.Parse(responseBody);

            var root = document.RootElement;

            if (root.TryGetProperty("output_text", out var outputText)
                && outputText.ValueKind == JsonValueKind.String)
            {
                return outputText.GetString() ?? string.Empty;
            }

            if (!root.TryGetProperty("output", out var output)
                || output.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            foreach (var outputItem in output.EnumerateArray())
            {
                if (!outputItem.TryGetProperty("content", out var content)
                    || content.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var contentItem in content.EnumerateArray())
                {
                    if (contentItem.TryGetProperty("type", out var type)
                        && type.GetString() == "output_text"
                        && contentItem.TryGetProperty("text", out var text)
                        && text.ValueKind == JsonValueKind.String)
                    {
                        builder.AppendLine(text.GetString());
                    }
                }
            }

            return builder.ToString().Trim();
        }

        private static string CleanJson(string value)
        {
            var text = value.Trim();

            if (text.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            {
                text = text[7..];
            }

            if (text.StartsWith("```", StringComparison.OrdinalIgnoreCase))
            {
                text = text[3..];
            }

            if (text.EndsWith("```", StringComparison.OrdinalIgnoreCase))
            {
                text = text[..^3];
            }

            return text.Trim();
        }

        private static ChatbotIntentDto LocalExtractIntent(string message)
        {
            var lower = message.ToLowerInvariant();

            var intent = new ChatbotIntentDto
            {
                IntentType = "product_advice",
                OriginalQuestion = message,
                BudgetType = lower.Contains("rẻ") || lower.Contains("tiết kiệm")
                    ? "cheapest"
                    : "balanced"
            };

            if (lower.Contains("nhật") || lower.Contains("japan"))
            {
                intent.CountryKeyword = "Nhật Bản";
                intent.CountryCode = "JP";
            }
            else if (lower.Contains("hàn") || lower.Contains("korea"))
            {
                intent.CountryKeyword = "Hàn Quốc";
                intent.CountryCode = "KR";
            }
            else if (lower.Contains("thái") || lower.Contains("thailand"))
            {
                intent.CountryKeyword = "Thái Lan";
                intent.CountryCode = "TH";
            }
            else if (lower.Contains("singapore"))
            {
                intent.CountryKeyword = "Singapore";
                intent.CountryCode = "SG";
            }
            else if (lower.Contains("trung quốc") || lower.Contains("china"))
            {
                intent.CountryKeyword = "Trung Quốc";
                intent.CountryCode = "CN";
            }

            intent.TravelDays = ExtractTravelDays(lower);

            var dataInfo = ExtractDataAmount(lower);

            if (dataInfo.Amount.HasValue)
            {
                intent.RequestedDataAmount = dataInfo.Amount;
                intent.RequestedDataUnit = dataInfo.Unit;
            }

            if (lower.Contains("không giới hạn") || lower.Contains("unlimited"))
            {
                intent.UsageLevel = "unlimited";
            }
            else if (lower.Contains("tiktok")
                     || lower.Contains("youtube")
                     || lower.Contains("video")
                     || lower.Contains("livestream")
                     || lower.Contains("dùng nhiều"))
            {
                intent.UsageLevel = "heavy";
            }
            else if (lower.Contains("ít") || lower.Contains("nhẹ"))
            {
                intent.UsageLevel = "light";
            }
            else
            {
                intent.UsageLevel = "normal";
            }

            if (lower.Contains("hotspot")
                || lower.Contains("phát mạng")
                || lower.Contains("chia sẻ mạng"))
            {
                intent.NeedsHotspot = true;
            }

            return intent;
        }

        private static int? ExtractTravelDays(string text)
        {
            var match = System.Text.RegularExpressions.Regex.Match(
                text,
                @"(\d+)\s*(ngày|day|days)");

            if (!match.Success)
                return null;

            return int.TryParse(match.Groups[1].Value, out var days)
                ? days
                : null;
        }

        private static (decimal? Amount, string? Unit) ExtractDataAmount(string text)
        {
            var match = System.Text.RegularExpressions.Regex.Match(
                text,
                @"(\d+([.,]\d+)?)\s*(gb|g|mb|m)\b",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (!match.Success)
                return (null, null);

            var rawAmount = match.Groups[1].Value.Replace(',', '.');

            if (!decimal.TryParse(
                    rawAmount,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var amount))
            {
                return (null, null);
            }

            var rawUnit = match.Groups[3].Value.ToLowerInvariant();

            var unit = rawUnit is "gb" or "g"
                ? "GB"
                : "MB";

            return (amount, unit);
        }

        private static string LocalGenerateAnswer(
            ChatbotIntentDto intent,
            IReadOnlyList<ChatbotProductSuggestionDto> suggestions)
        {
            if (suggestions.Count == 0)
            {
                return "Tôi chưa tìm thấy gói eSIM phù hợp trong hệ thống. Bạn vui lòng cho biết quốc gia đến, số ngày đi và nhu cầu dùng data để tôi tư vấn chính xác hơn.";
            }

            var builder = new StringBuilder();

            builder.AppendLine("Dựa trên nhu cầu của bạn, tôi gợi ý các gói eSIM sau:");

            for (var i = 0; i < suggestions.Count; i++)
            {
                var item = suggestions[i];

                var dataText = GetDataText(item);

                builder.AppendLine();
                builder.AppendLine($"{i + 1}. {item.PackageName}");
                builder.AppendLine($"- Quốc gia: {item.CountryName}");
                builder.AppendLine($"- Dung lượng: {dataText}");
                builder.AppendLine($"- Thời hạn: {item.ValidityDays} ngày");
                builder.AppendLine($"- Giá: {item.SalePrice:N0} {item.Currency}");
                builder.AppendLine($"- Link mua: {item.BuyUrl}");
            }

            builder.AppendLine();
            builder.AppendLine("Bạn có thể chọn gói phù hợp nhất rồi bấm Mua ngay để thanh toán và nhận QR eSIM qua email.");

            return builder.ToString();
        }

        private static string GetDataText(ChatbotProductSuggestionDto item)
        {
            if (item.IsUnlimited)
                return "Không giới hạn";

            if (!item.DataAmount.HasValue)
                return "Theo chính sách gói";

            if (string.IsNullOrWhiteSpace(item.DataUnit))
                return $"{item.DataAmount.Value:N0}";

            return $"{item.DataAmount.Value:N0} {item.DataUnit}";
        }
    }
}
