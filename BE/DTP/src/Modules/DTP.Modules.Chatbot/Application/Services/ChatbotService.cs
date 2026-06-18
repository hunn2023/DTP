using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Domain.Enums;
using DTP.Modules.Chatbot.Application.Abstractions;
using DTP.Modules.Chatbot.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Application.Services
{
    public sealed class ChatbotService : IChatbotService
    {
        private const string ModuleName = "Chatbot";

        private readonly IChatbotAiClient _aiClient;
        private readonly IChatbotCatalogReader _catalogReader;
        private readonly IAuditLogWriter _auditLogWriter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ChatbotService> _logger;
        private readonly IChatbotRateLimitService _rateLimitService;
        public ChatbotService(
            IChatbotAiClient aiClient,
            IChatbotCatalogReader catalogReader,
            IAuditLogWriter auditLogWriter,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChatbotService> logger,
            IChatbotRateLimitService rateLimitService)
        {
            _aiClient = aiClient;
            _catalogReader = catalogReader;
            _auditLogWriter = auditLogWriter;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _rateLimitService = rateLimitService;
        }

        public async Task<ChatResponseDto> SendMessageAsync(
            ChatRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var sessionId = EnsureSessionId(request?.SessionId);

            try
            {
                if (request == null)
                {
                    await WriteAuditSafeAsync(
                        action: "Chatbot Message Failed",
                        actionType: AuditActionType.Create,
                        status: AuditStatus.Failed,
                        entityName: "ChatbotMessage",
                        description: "Chatbot request is null.",
                        newValues: new
                        {
                            SessionId = sessionId,
                            Reason = "Request is null",
                            Request = GetRequestAuditInfo()
                        },
                        errorMessage: "Dữ liệu gửi lên không hợp lệ.",
                        cancellationToken: cancellationToken);

                    return new ChatResponseDto
                    {
                        SessionId = sessionId,
                        Message = "Dữ liệu gửi lên không hợp lệ.",
                        NeedMoreInfo = true,
                        MissingFields = new List<string> { "request" }
                    };
                }

                var message = request.Message?.Trim();

                if (string.IsNullOrWhiteSpace(message))
                {
                    await WriteAuditSafeAsync(
                        action: "Chatbot Message Failed",
                        actionType: AuditActionType.Create,
                        status: AuditStatus.Failed,
                        entityName: "ChatbotMessage",
                        description: "Chatbot message is empty.",
                        newValues: new
                        {
                            SessionId = sessionId,
                            Reason = "Message is empty",
                            Request = GetRequestAuditInfo()
                        },
                        errorMessage: "Bạn vui lòng nhập nội dung cần tư vấn.",
                        cancellationToken: cancellationToken);

                    return new ChatResponseDto
                    {
                        SessionId = sessionId,
                        Message = "Bạn vui lòng nhập nội dung cần tư vấn.",
                        NeedMoreInfo = true,
                        MissingFields = new List<string> { "message" }
                    };
                }

                var rateLimitResult = await _rateLimitService.CheckAsync(
                                    new ChatbotRateLimitContext
                                    {
                                        IpAddress = GetClientIp(),
                                        //UserId = Guid.NewGuid(), //GetCurrentUserId(),
                                        SessionId = sessionId,
                                        Message = message
                                    },
                                    cancellationToken);

                if (!rateLimitResult.Allowed)
                {
                    await WriteAuditSafeAsync(
                        action: "Chatbot Rate Limit Exceeded",
                        actionType: AuditActionType.Create,
                        status: AuditStatus.Failed,
                        entityName: "ChatbotMessage",
                        description: "Chatbot request was blocked by rate limit.",
                        newValues: new
                        {
                            SessionId = sessionId,
                            Message = message,
                            Reason = rateLimitResult.Reason,
                            RetryAfterSeconds = rateLimitResult.RetryAfterSeconds,
                            Request = GetRequestAuditInfo()
                        },
                        errorMessage: rateLimitResult.Message,
                        cancellationToken: cancellationToken);

                    return new ChatResponseDto
                    {
                        SessionId = sessionId,
                        Message = rateLimitResult.Message,
                        NeedMoreInfo = false,
                        MissingFields = new List<string>(),
                        Suggestions = new List<ChatbotProductSuggestionDto>()
                    };
                }


                await WriteAuditSafeAsync(
                    action: "Chatbot Message Received",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Success,
                    entityName: "ChatbotMessage",
                    description: "Customer sent a chatbot message.",
                    newValues: new
                    {
                        SessionId = sessionId,
                        Message = message,
                        Request = GetRequestAuditInfo()
                    },
                    cancellationToken: cancellationToken);

                ChatbotIntentDto intent;

                try
                {
                    intent = await _aiClient.ExtractIntentAsync(
                        message,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Extract chatbot intent failed.");

                    await WriteAuditSafeAsync(
                        action: "Extract Chatbot Intent Failed",
                        actionType: AuditActionType.Create,
                        status: AuditStatus.Failed,
                        entityName: "ChatbotIntent",
                        description: "Extract chatbot intent failed.",
                        newValues: new
                        {
                            SessionId = sessionId,
                            Message = message,
                            Request = GetRequestAuditInfo()
                        },
                        errorMessage: ex.Message,
                        cancellationToken: cancellationToken);

                    intent = new ChatbotIntentDto
                    {
                        IntentType = "unknown",
                        OriginalQuestion = message
                    };
                }

                intent.OriginalQuestion ??= message;

                if (!IsProductAdvice(intent))
                {
                    var unknownMessage =
                        "Tôi có thể tư vấn gói eSIM theo quốc gia, số ngày đi và nhu cầu dùng data. " +
                        "Ví dụ: Tôi đi Nhật Bản 7 ngày, dùng TikTok nhiều.";

                    return new ChatResponseDto
                    {
                        SessionId = sessionId,
                        Message = unknownMessage,
                        NeedMoreInfo = true,
                        MissingFields = new List<string>
                            {
                                "country",
                                "travelDays",
                                "usageLevel"
                            },
                        Intent = intent,
                        Suggestions = new List<ChatbotProductSuggestionDto>()
                    };
                }

                var missingFields = GetMissingFields(intent);

                if (missingFields.Count > 0)
                {
                    var needMoreInfoMessage = BuildNeedMoreInfoMessage(missingFields);

                    await WriteAuditSafeAsync(
                        action: "Chatbot Need More Info",
                        actionType: AuditActionType.Create,
                        status: AuditStatus.Failed,
                        entityName: "ChatbotMessage",
                        description: "Chatbot needs full information in one message before searching products.",
                        newValues: new
                        {
                            SessionId = sessionId,
                            Message = message,
                            Intent = intent,
                            MissingFields = missingFields,
                            Request = GetRequestAuditInfo()
                        },
                        errorMessage: needMoreInfoMessage,
                        cancellationToken: cancellationToken);

                    return new ChatResponseDto
                    {
                        SessionId = sessionId,
                        Message = needMoreInfoMessage,
                        NeedMoreInfo = true,
                        MissingFields = missingFields,
                        Intent = intent,
                        Suggestions = new List<ChatbotProductSuggestionDto>()
                    };
                }

                var suggestions = new List<ChatbotProductSuggestionDto>();

                try
                {
                    suggestions = (await _catalogReader.SearchEsimPackagesAsync(
                            intent,
                            take: 3,
                            cancellationToken))
                        .ToList();

                    if (suggestions.Count == 0)
                    {
                        await WriteAuditSafeAsync(
                            action: "Search Chatbot Product No Result",
                            actionType: AuditActionType.Create,
                            status: AuditStatus.Failed,
                            entityName: "EsimPackage",
                            description: "No eSIM package found for chatbot intent.",
                            newValues: new
                            {
                                SessionId = sessionId,
                                Message = message,
                                Intent = intent,
                                Request = GetRequestAuditInfo()
                            },
                            errorMessage: "Không tìm thấy gói eSIM phù hợp.",
                            cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await WriteAuditSafeAsync(
                            action: "Search Chatbot Product Success",
                            actionType: AuditActionType.Create,
                            status: AuditStatus.Success,
                            entityName: "EsimPackage",
                            description: "Found eSIM packages for chatbot intent.",
                            newValues: new
                            {
                                SessionId = sessionId,
                                Message = message,
                                Intent = intent,
                                SuggestionCount = suggestions.Count,
                                Suggestions = suggestions.Select(x => new
                                {
                                    x.ProductId,
                                    x.ProductVariantId,
                                    x.EsimPackageId,
                                    x.ProductName,
                                    x.PackageName,
                                    x.CountryName,
                                    x.ValidityDays,
                                    x.DataAmount,
                                    x.DataUnit,
                                    x.IsUnlimited,
                                    x.SalePrice,
                                    x.Currency,
                                    x.Score,
                                    x.BuyUrl
                                }),
                                Request = GetRequestAuditInfo()
                            },
                            cancellationToken: cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Search chatbot products failed.");

                    await WriteAuditSafeAsync(
                        action: "Search Chatbot Product Failed",
                        actionType: AuditActionType.Create,
                        status: AuditStatus.Failed,
                        entityName: "EsimPackage",
                        description: "Search eSIM packages for chatbot failed.",
                        newValues: new
                        {
                            SessionId = sessionId,
                            Message = message,
                            Intent = intent,
                            Request = GetRequestAuditInfo()
                        },
                        errorMessage: ex.Message,
                        cancellationToken: cancellationToken);

                    suggestions = new List<ChatbotProductSuggestionDto>();
                }

                string answer;

                if (suggestions.Count == 0)
                {
                    answer = "Tôi chưa tìm thấy gói eSIM phù hợp. Bạn vui lòng kiểm tra lại quốc gia, số ngày đi hoặc nhu cầu dung lượng.";
                }
                else
                {
                    try
                    {
                        answer = await _aiClient.GenerateAnswerAsync(
                            message,
                            intent,
                            suggestions,
                            cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Generate chatbot answer failed.");

                        await WriteAuditSafeAsync(
                            action: "Generate Chatbot Answer Failed",
                            actionType: AuditActionType.Create,
                            status: AuditStatus.Failed,
                            entityName: "ChatbotMessage",
                            description: "Generate chatbot answer failed.",
                            newValues: new
                            {
                                SessionId = sessionId,
                                Message = message,
                                Intent = intent,
                                SuggestionCount = suggestions.Count,
                                Request = GetRequestAuditInfo()
                            },
                            errorMessage: ex.Message,
                            cancellationToken: cancellationToken);

                        answer = BuildFallbackAnswer(suggestions);
                    }
                }

             

                var response = new ChatResponseDto
                {
                    SessionId = sessionId,
                    Message = answer,
                    NeedMoreInfo = false,
                    MissingFields = new List<string>(),
                    Intent = intent,
                    Suggestions = suggestions
                };

                await WriteAuditSafeAsync(
                    action: "Chatbot Message Success",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Success,
                    entityName: "ChatbotMessage",
                    description: "Chatbot processed customer message successfully.",
                    newValues: new
                    {
                        SessionId = sessionId,
                        Message = message,
                        Intent = intent,
                        NeedMoreInfo = response.NeedMoreInfo,
                        MissingFields = response.MissingFields,
                        SuggestionCount = suggestions.Count,
                        Suggestions = suggestions.Select(x => new
                        {
                            x.ProductId,
                            x.ProductVariantId,
                            x.EsimPackageId,
                            x.ProductName,
                            x.PackageName,
                            x.CountryName,
                            x.ValidityDays,
                            x.DataAmount,
                            x.DataUnit,
                            x.IsUnlimited,
                            x.SalePrice,
                            x.Currency,
                            x.Score,
                            x.BuyUrl
                        }),
                        Request = GetRequestAuditInfo()
                    },
                    cancellationToken: cancellationToken);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chatbot message failed.");

                await WriteAuditSafeAsync(
                    action: "Chatbot Message Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "ChatbotMessage",
                    description: "Chatbot message processing failed.",
                    newValues: new
                    {
                        SessionId = sessionId,
                        Request = request,
                        HttpRequest = GetRequestAuditInfo()
                    },
                    errorMessage: ex.Message,
                    cancellationToken: cancellationToken);

                return new ChatResponseDto
                {
                    SessionId = sessionId,
                    Message = "Xin lỗi, hiện tại chatbot chưa thể xử lý yêu cầu này. Bạn vui lòng thử lại sau.",
                    NeedMoreInfo = false,
                    MissingFields = new List<string>(),
                    Suggestions = new List<ChatbotProductSuggestionDto>()
                };
            }
        }

        private async Task WriteAuditSafeAsync(
            string action,
            AuditActionType actionType,
            AuditStatus status,
            string? entityName = null,
            Guid? entityId = null,
            string? description = null,
            object? oldValues = null,
            object? newValues = null,
            string? errorMessage = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _auditLogWriter.WriteAsync(
                    module: ModuleName,
                    action: action,
                    actionType: actionType,
                    status: status,
                    entityName: entityName,
                    entityId: entityId,
                    description: description,
                    oldValues: oldValues,
                    newValues: newValues,
                    errorMessage: errorMessage,
                    cancellationToken: cancellationToken);
            }
            catch
            {
                // Không được để lỗi audit làm fail nghiệp vụ Chatbot.
            }
        }

        private object GetRequestAuditInfo()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return new
                {
                    IpAddress = (string?)null,
                    UserAgent = (string?)null,
                    Path = (string?)null,
                    Method = (string?)null
                };
            }

            return new
            {
                IpAddress = GetClientIp(),
                UserAgent = GetUserAgent(),
                Path = httpContext.Request.Path.Value,
                Method = httpContext.Request.Method
            };
        }

        private string? GetClientIp()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                return null;

            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                return forwardedFor
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault()
                    ?.Trim();
            }

            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(realIp))
                return realIp.Trim();

            return httpContext.Connection.RemoteIpAddress?.ToString();
        }

        private string? GetUserAgent()
        {
            return _httpContextAccessor.HttpContext?
                .Request
                .Headers["User-Agent"]
                .FirstOrDefault();
        }


        private string? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                return null;

            return user.FindFirst("sub")?.Value
                   ?? user.FindFirst("userId")?.Value
                   ?? user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        private static bool IsProductAdvice(ChatbotIntentDto intent)
        {
            if (intent == null)
                return false;

            var question = intent.OriginalQuestion?
                .Trim()
                .ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(question))
                return false;

            var hasProductKeyword =
                question.Contains("esim")
                || question.Contains("sim")
                || question.Contains("gói")
                || question.Contains("mua")
                || question.Contains("du lịch")
                || question.Contains("tư vấn")
                || question.Contains("data")
                || question.Contains("gb")
                || question.Contains("mb")
                || question.Contains("không giới hạn")
                || question.Contains("unlimited");

            var hasCountry =
                !string.IsNullOrWhiteSpace(intent.CountryKeyword)
                || !string.IsNullOrWhiteSpace(intent.CountryCode);

            var hasUsefulInfo =
                hasProductKeyword
                || hasCountry
                || intent.TravelDays.HasValue
                || intent.RequestedDataAmount.HasValue;

            if (!hasUsefulInfo)
                return false;

            return string.Equals(intent.IntentType, "product_advice", StringComparison.OrdinalIgnoreCase)
                   || hasUsefulInfo;
        }

        private static List<string> GetMissingFields(ChatbotIntentDto intent)
        {
            var result = new List<string>();

            if (!IsProductAdvice(intent))
                return result;

            if (string.IsNullOrWhiteSpace(intent.CountryKeyword)
                && string.IsNullOrWhiteSpace(intent.CountryCode))
            {
                result.Add("country");
            }

            if (!intent.TravelDays.HasValue || intent.TravelDays.Value <= 0)
            {
                result.Add("travelDays");
            }

            if (!HasUsageInfo(intent))
            {
                result.Add("usageLevel");
            }

            return result;
        }

        private static bool HasUsageInfo(ChatbotIntentDto intent)
        {
            if (intent.RequestedDataAmount.HasValue)
                return true;

            var usageLevel = intent.UsageLevel?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(usageLevel))
                return false;

            return usageLevel is "light" or "normal" or "heavy" or "unlimited";
        }

        private static string BuildNeedMoreInfoMessage(IReadOnlyList<string> missingFields)
        {
            return "Để tư vấn chính xác, bạn vui lòng gửi đầy đủ trong 1 câu gồm: quốc gia muốn đi, số ngày sử dụng và nhu cầu data. Ví dụ: Tôi đi Nhật Bản 7 ngày, dùng TikTok nhiều.";
        }


        private static string BuildFallbackAnswer(
              IReadOnlyList<ChatbotProductSuggestionDto> suggestions)
        {
            if (suggestions.Count == 0)
            {
                return "Tôi chưa tìm thấy gói eSIM phù hợp. Bạn vui lòng cho biết quốc gia đến, số ngày đi và nhu cầu dùng data để tôi tư vấn chính xác hơn.";
            }

            var first = suggestions.First();

            var dataText = GetDataText(first);

            return $"Tôi gợi ý bạn tham khảo gói {first.PackageName}, dung lượng {dataText}, thời hạn {first.ValidityDays} ngày, giá {first.SalePrice:N0} {first.Currency}.";
        }

        private static string EnsureSessionId(string? sessionId)
        {
            return string.IsNullOrWhiteSpace(sessionId)
                ? Guid.NewGuid().ToString("N")
                : sessionId.Trim();
        }

        private static string GetDataText(ChatbotProductSuggestionDto item)
        {
            if (item.IsUnlimited)
                return "không giới hạn";

            if (!item.DataAmount.HasValue)
                return "theo chính sách gói";

            if (string.IsNullOrWhiteSpace(item.DataUnit))
                return $"{item.DataAmount.Value:N0}";

            return $"{item.DataAmount.Value:N0} {item.DataUnit}";
        }
    }


}