using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public class VnptEpayRegisterVaRequest
    {
        public long Amount { get; set; }

        public string CustomerName { get; set; } = default!;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        /// <summary>
        /// Mã tham chiếu nội bộ: OrderId, OrderCode hoặc PaymentTransactionId
        /// </summary>
        public string ReferenceId { get; set; } = default!;

        /// <summary>
        /// Nội dung chuyển khoản / nội dung QR
        /// </summary>
        public string ContentQr { get; set; } = default!;
    }

    public class VnptEpayRegisterVaResponse
    {
        [JsonPropertyName("response_code")]
        public string? ResponseCode { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("account_no")]
        public string? AccountNo { get; set; }

        [JsonPropertyName("account_name")]
        public string? AccountName { get; set; }

        [JsonPropertyName("bank_code")]
        public string? BankCode { get; set; }

        [JsonPropertyName("bank_name")]
        public string? BankName { get; set; }

        [JsonPropertyName("map_id")]
        public string? MapId { get; set; }

        [JsonPropertyName("qr_code")]
        public string? QrCode { get; set; }

        [JsonPropertyName("qr_dataRaw")]
        public string? QrDataRaw { get; set; }

        [JsonPropertyName("qr_url")]
        public string? QrUrl { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        public bool IsSuccess =>
            ResponseCode == "00" ||
            ResponseCode == "0";
    }
}
