using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Domain.Enums;
using DTP.Modules.Chatbot.Application.Abstractions;
using DTP.Modules.Chatbot.Application.DTOs;
using DTP.Modules.Chatbot.Application.Enums;
using DTP.Modules.Knowledge.Application.Abstractions;
using DTP.Modules.Knowledge.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
        private readonly IKnowledgeSearchService _knowledgeSearchService;
        public ChatbotService(
            IChatbotAiClient aiClient,
            IChatbotCatalogReader catalogReader,
            IAuditLogWriter auditLogWriter,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChatbotService> logger,
            IChatbotRateLimitService rateLimitService,
             IKnowledgeSearchService knowledgeSearchService
            )
        {
            _aiClient = aiClient;
            _catalogReader = catalogReader;
            _auditLogWriter = auditLogWriter;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _rateLimitService = rateLimitService;
            _knowledgeSearchService = knowledgeSearchService;
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

                ApplySmartDefaults(intent);

                var route = DetermineRoute(message, intent);

                if (route == ChatbotRouteType.Knowledge)
                {
                    return await HandleKnowledgeAsync(
                        sessionId,
                        message,
                        intent,
                        cancellationToken);
                }

                if (route == ChatbotRouteType.Mixed)
                {
                    return await HandleMixedAsync(
                        sessionId,
                        message,
                        intent,
                        cancellationToken);
                }

                if (!IsProductAdvice(intent))
                {
                    var unknownMessage =
                        "Bạn đang cần tư vấn gói eSIM hay cần hỗ trợ thông tin/cách cài đặt ạ? " +
                        "Ví dụ: Tôi đi Thái Lan 5 ngày, hoặc: Hướng dẫn cài eSIM trên iPhone.";

                    return new ChatResponseDto
                    {
                        SessionId = sessionId,
                        Message = unknownMessage,
                        NeedMoreInfo = true,
                        MissingFields = new List<string>(),
                        Intent = intent,
                        Suggestions = new List<ChatbotProductSuggestionDto>()
                    };
                }


                var blockingMissingFields = GetBlockingMissingFields(intent);

                if (blockingMissingFields.Count > 0)
                {
                    var needMoreInfoMessage =
                        "Bạn vui lòng cho biết quốc gia muốn đi để tôi tư vấn gói eSIM phù hợp. " +
                        "Ví dụ: Tôi đi Thái Lan, nên mua gói nào?";

                    await WriteAuditSafeAsync(
                        action: "Chatbot Need More Info",
                        actionType: AuditActionType.Create,
                        status: AuditStatus.Failed,
                        entityName: "ChatbotMessage",
                        description: "Chatbot needs country before searching products.",
                        newValues: new
                        {
                            SessionId = sessionId,
                            Message = message,
                            Intent = intent,
                            MissingFields = blockingMissingFields,
                            Request = GetRequestAuditInfo()
                        },
                        errorMessage: needMoreInfoMessage,
                        cancellationToken: cancellationToken);

                    return new ChatResponseDto
                    {
                        SessionId = sessionId,
                        Message = needMoreInfoMessage,
                        NeedMoreInfo = true,
                        MissingFields = blockingMissingFields,
                        Intent = intent,
                        Suggestions = new List<ChatbotProductSuggestionDto>()
                    };
                }

                var optionalMissingFields = GetOptionalMissingFields(intent);

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
                    NeedMoreInfo = suggestions.Count == 0,
                    MissingFields = suggestions.Count > 0
                        ? optionalMissingFields
                        : blockingMissingFields,
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
                //UserAgent = GetUserAgent(),
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
                || question.Contains("unlimited")
                || question.Contains("nên mua")
                || question.Contains("gói nào");

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

        private static string BuildNeedMoreInfoMessage(IReadOnlyList<string> missingFields)
        {
            if (missingFields.Contains("country"))
            {
                return "Bạn vui lòng cho biết quốc gia muốn đi để tôi tư vấn gói eSIM phù hợp. Ví dụ: Tôi đi Thái Lan, nên mua gói nào?";
            }

            return "Bạn vui lòng cung cấp thêm thông tin để tôi tư vấn gói eSIM phù hợp hơn.";
        }

        private static List<string> GetBlockingMissingFields(ChatbotIntentDto intent)
        {
            var result = new List<string>();

            if (!IsProductAdvice(intent))
                return result;

            if (string.IsNullOrWhiteSpace(intent.CountryKeyword)
                && string.IsNullOrWhiteSpace(intent.CountryCode))
            {
                result.Add("country");
            }

            return result;
        }

        private static List<string> GetOptionalMissingFields(ChatbotIntentDto intent)
        {
            var result = new List<string>();

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

            return usageLevel is "light" or "normal" or "heavy" or "unlimited";
        }

        private static void ApplySmartDefaults(ChatbotIntentDto intent)
        {
            var hasCountry =
                !string.IsNullOrWhiteSpace(intent.CountryKeyword)
                || !string.IsNullOrWhiteSpace(intent.CountryCode);

            var hasUsage =
                !string.IsNullOrWhiteSpace(intent.UsageLevel)
                || intent.RequestedDataAmount.HasValue;

            // Chỉ cần có quốc gia thì vẫn tư vấn được.
            // Nếu khách chưa nói nhu cầu data thì mặc định là normal.
            if (hasCountry && !hasUsage)
            {
                intent.UsageLevel = "normal";
            }

            if (string.IsNullOrWhiteSpace(intent.BudgetType))
            {
                intent.BudgetType = "balanced";
            }
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



        private static ChatbotRouteType DetermineRoute(
            string message,
            ChatbotIntentDto intent)
        {
            var isKnowledge = IsKnowledgeQuestion(message);
            var isProduct = IsProductSearchQuestion(message, intent);

            if (isKnowledge && isProduct)
                return ChatbotRouteType.Mixed;

            if (isKnowledge)
                return ChatbotRouteType.Knowledge;

            if (isProduct)
                return ChatbotRouteType.Product;

            return ChatbotRouteType.Unknown;
        }

        private static bool IsKnowledgeQuestion(string message)
        {
            var text = NormalizeText(message);

            var keywords = new[]
            {
        "esim là gì",
        "hướng dẫn",
        "cài esim",
        "cài đặt esim",
        "cài đặt",
        "kích hoạt",
        "active esim",
        "quét qr",
        "mã qr",
        "lỗi qr",
        "không quét được",
        "không nhận được qr",
        "không nhận được email",
        "iphone",
        "android",
        "samsung",
        "đổi điện thoại",
        "dùng lại được không",
        "dùng được mấy lần",
        "hoàn tiền",
        "đổi trả",
        "bảo hành",
        "chính sách",
        "huỷ đơn",
        "hủy đơn",
        "thanh toán xong",
        "bao lâu nhận",
        "xuất hoá đơn",
        "xuất hóa đơn",
        "vat",
        "có cần cccd",
        "có cần kyc",
        "làm sao",
        "như nào"
    };

            return keywords.Any(text.Contains);
        }


        private static bool IsProductSearchQuestion(
            string message,
            ChatbotIntentDto intent)
        {
            var text = NormalizeText(message);

            var hasCountry =
                !string.IsNullOrWhiteSpace(intent.CountryKeyword)
                || !string.IsNullOrWhiteSpace(intent.CountryCode);

            var hasProductKeyword =
                text.Contains("mua")
                || text.Contains("gói esim")
                || text.Contains("gói nào")
                || text.Contains("nên mua")
                || text.Contains("tư vấn")
                || text.Contains("giá")
                || text.Contains("bao nhiêu tiền")
                || text.Contains("data")
                || text.Contains("gb")
                || text.Contains("mb")
                || text.Contains("dung lượng")
                || text.Contains("không giới hạn")
                || text.Contains("unlimited")
                || text.Contains("du lịch")
                || text.Contains("đi ");

            return string.Equals(intent.IntentType, "product_advice", StringComparison.OrdinalIgnoreCase)
                   || hasCountry
                   || hasProductKeyword
                   || intent.TravelDays.HasValue
                   || intent.RequestedDataAmount.HasValue;
        }

        private static string NormalizeText(string input)
        {
            return (input ?? string.Empty)
                .Trim()
                .ToLowerInvariant();
        }

        private async Task<ChatResponseDto> HandleKnowledgeAsync(
    string sessionId,
    string message,
    ChatbotIntentDto intent,
    CancellationToken cancellationToken)
        {
            List<KnowledgeSearchResultDto> knowledgeResults;

            try
            {
                knowledgeResults = await _knowledgeSearchService.SearchAsync(
                    message,
                    topK: 5,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search chatbot knowledge failed.");

                await WriteAuditSafeAsync(
                    action: "Search Chatbot Knowledge Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "KnowledgeChunk",
                    description: "Search knowledge for chatbot failed.",
                    newValues: new
                    {
                        SessionId = sessionId,
                        Message = message,
                        Intent = intent,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: ex.Message,
                    cancellationToken: cancellationToken);

                return new ChatResponseDto
                {
                    SessionId = sessionId,
                    Message = "Xin lỗi, hiện tại tôi chưa tra cứu được thông tin này. Bạn vui lòng thử lại sau.",
                    NeedMoreInfo = false,
                    MissingFields = new List<string>(),
                    Intent = intent,
                    Suggestions = new List<ChatbotProductSuggestionDto>()
                };
            }

            if (knowledgeResults.Count == 0)
            {
                return new ChatResponseDto
                {
                    SessionId = sessionId,
                    Message = "Tôi chưa tìm thấy thông tin chính xác trong dữ liệu hiện có. Bạn có thể nói rõ hơn câu hỏi để tôi kiểm tra lại không?",
                    NeedMoreInfo = true,
                    MissingFields = new List<string>(),
                    Intent = intent,
                    Suggestions = new List<ChatbotProductSuggestionDto>()
                };
            }

            string answer;

            try
            {
                answer = await _aiClient.GenerateKnowledgeAnswerAsync(
                    message,
                    knowledgeResults,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generate chatbot knowledge answer failed.");

                await WriteAuditSafeAsync(
                    action: "Generate Chatbot Knowledge Answer Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "ChatbotMessage",
                    description: "Generate knowledge answer failed.",
                    newValues: new
                    {
                        SessionId = sessionId,
                        Message = message,
                        Intent = intent,
                        KnowledgeCount = knowledgeResults.Count,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: ex.Message,
                    cancellationToken: cancellationToken);

                answer = BuildKnowledgeFallbackAnswer(knowledgeResults);
            }

            await WriteAuditSafeAsync(
                action: "Chatbot Knowledge Message Success",
                actionType: AuditActionType.Create,
                status: AuditStatus.Success,
                entityName: "KnowledgeChunk",
                description: "Chatbot answered by knowledge.",
                newValues: new
                {
                    SessionId = sessionId,
                    Message = message,
                    Intent = intent,
                    KnowledgeCount = knowledgeResults.Count,
                    KnowledgeResults = knowledgeResults.Select(x => new
                    {
                        x.Id,
                        x.SourceType,
                        x.SourceId,
                        x.Title,
                        x.SourceUrl,
                        x.Score
                    }),
                    Request = GetRequestAuditInfo()
                },
                cancellationToken: cancellationToken);

            return new ChatResponseDto
            {
                SessionId = sessionId,
                Message = answer,
                NeedMoreInfo = false,
                MissingFields = new List<string>(),
                Intent = intent,
                Suggestions = new List<ChatbotProductSuggestionDto>()
            };
        }

        private async Task<ChatResponseDto> HandleMixedAsync(
    string sessionId,
    string message,
    ChatbotIntentDto intent,
    CancellationToken cancellationToken)
        {
            var blockingMissingFields = GetBlockingMissingFields(intent);

            var suggestions = new List<ChatbotProductSuggestionDto>();
            var knowledgeResults = new List<KnowledgeSearchResultDto>();

            if (blockingMissingFields.Count == 0)
            {
                try
                {
                    suggestions = (await _catalogReader.SearchEsimPackagesAsync(
                            intent,
                            take: 3,
                            cancellationToken))
                        .ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Search chatbot products for mixed question failed.");
                }
            }

            try
            {
                knowledgeResults = await _knowledgeSearchService.SearchAsync(
                    message,
                    topK: 3,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search chatbot knowledge for mixed question failed.");
            }

            if (suggestions.Count == 0 && knowledgeResults.Count == 0)
            {
                return new ChatResponseDto
                {
                    SessionId = sessionId,
                    Message = "Bạn vui lòng cho biết thêm điểm đến, số ngày đi hoặc vấn đề cần hỗ trợ để tôi tư vấn chính xác hơn.",
                    NeedMoreInfo = true,
                    MissingFields = blockingMissingFields,
                    Intent = intent,
                    Suggestions = new List<ChatbotProductSuggestionDto>()
                };
            }

            string answer;

            try
            {
                answer = await _aiClient.GenerateMixedAnswerAsync(
                    message,
                    intent,
                    suggestions,
                    knowledgeResults,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generate chatbot mixed answer failed.");

                if (suggestions.Count > 0)
                {
                    answer = BuildFallbackAnswer(suggestions);
                }
                else
                {
                    answer = BuildKnowledgeFallbackAnswer(knowledgeResults);
                }
            }

            return new ChatResponseDto
            {
                SessionId = sessionId,
                Message = answer,
                NeedMoreInfo = suggestions.Count == 0 && blockingMissingFields.Count > 0,
                MissingFields = suggestions.Count > 0
                    ? GetOptionalMissingFields(intent)
                    : blockingMissingFields,
                Intent = intent,
                Suggestions = suggestions
            };
        }

        private static string BuildKnowledgeFallbackAnswer(
    IReadOnlyList<KnowledgeSearchResultDto> knowledgeResults)
        {
            if (knowledgeResults.Count == 0)
            {
                return "Tôi chưa tìm thấy thông tin chính xác trong dữ liệu hiện có.";
            }

            var first = knowledgeResults.First();

            var content = first.Content;

            if (content.Length > 600)
            {
                content = content[..600] + "...";
            }

            if (!string.IsNullOrWhiteSpace(first.SourceUrl))
            {
                return $"{content}\n\nBạn có thể xem thêm tại: {first.SourceUrl}";
            }

            return content;
        }
    }
}