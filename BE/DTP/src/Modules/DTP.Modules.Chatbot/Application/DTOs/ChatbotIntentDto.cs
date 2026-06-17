using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Application.DTOs
{
    public sealed class ChatbotIntentDto
    {
        /// <summary>
        /// product_advice | faq | order_support | unknown
        /// </summary>
        public string IntentType { get; set; } = "unknown";

        /// <summary>
        /// Ví dụ: Nhật Bản, Japan, Thái Lan, Thailand.
        /// </summary>
        public string? CountryKeyword { get; set; }

        /// <summary>
        /// ISO code nếu AI suy luận được: JP, KR, TH, SG...
        /// </summary>
        public string? CountryCode { get; set; }

        public int? TravelDays { get; set; }

        /// <summary>
        /// light | normal | heavy | unlimited
        /// </summary>
        public string? UsageLevel { get; set; }

        /// <summary>
        /// cheapest | balanced | premium
        /// </summary>
        public string? BudgetType { get; set; }

        public bool? NeedsHotspot { get; set; }

        public bool? NeedsPhoneNumber { get; set; }

        public bool? NeedsSms { get; set; }

        public string? OriginalQuestion { get; set; }

        public decimal? RequestedDataAmount { get; set; }

        public string? RequestedDataUnit { get; set; }
    }
}
