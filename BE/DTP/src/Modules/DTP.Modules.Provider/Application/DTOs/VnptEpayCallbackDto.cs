using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class VnptEpayCallbackDto
    {
        public string? RequestId { get; set; }
        public string? RequestTime { get; set; }
        public string? ReferenceId { get; set; }
        public string? MapId { get; set; }

        public decimal Amount { get; set; }
        public decimal Fee { get; set; }

        public string? VaAcc { get; set; }
        public string? VaName { get; set; }
        public string? MerchantCode { get; set; }
        public string? Signature { get; set; }

        // Không bắt buộc, chỉ để tương thích code cũ nếu cần
        public string? ProviderTransactionId => ReferenceId;
    }
}
