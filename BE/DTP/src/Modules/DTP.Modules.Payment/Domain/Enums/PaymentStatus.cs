using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Domain.Enums
{
    public enum PaymentStatus
    {
        CreatingQr = 0,
        Pending = 1,
        Processing = 2,
        Paid = 3,
        Failed = 4,
        Cancelled = 5,
        Expired = 6,
        Refunded = 7
    }
}
