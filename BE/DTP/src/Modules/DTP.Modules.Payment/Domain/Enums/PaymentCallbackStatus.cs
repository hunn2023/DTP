using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Domain.Enums
{
    public enum PaymentCallbackStatus
    {
        Received = 1,
        Verified = 2,
        InvalidSignature = 3,
        Processed = 4,
        Failed = 5,
        Duplicated = 6,
        Ignored = 7
    }
}
