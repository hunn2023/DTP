using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Domain.Enums
{
    public enum PaymentTransactionStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3,
        Expired = 4,
        Cancelled = 5,
        Refunded = 6
    }
}
