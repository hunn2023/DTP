using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{

    public class VnptEpayCallbackDto
    {
        public string RequestId { get; set; }
        public string RequestTime { get; set; }
        public string BankTranTime { get; set; }
        public string ReferenceId { get; set; }
        public string MapId { get; set; }
        public long Amount { get; set; }
        public string Signature { get; set; }
        public string MerchantCode { get; set; }
        public long Fee { get; set; }
        public string VaName { get; set; }
        public string VaAcc { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string Remark { get; set; }
    }
}
