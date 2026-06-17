using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{

    public class VnptEpayCallbackDto
    {
        public required string RequestId { get; set; }
        public required string RequestTime { get; set; }
        public required string BankTranTime { get; set; }
        public required string ReferenceId { get; set; }
        public required string MapId { get; set; }
        public long Amount { get; set; }
        public required string Signature { get; set; }
        public required string MerchantCode { get; set; }
        public long Fee { get; set; }
        public required string VaName { get; set; }
        public required string VaAcc { get; set; }
        public required string BankCode { get; set; }
        public required string BankName { get; set; }
        public required string Remark { get; set; }
    }
}
