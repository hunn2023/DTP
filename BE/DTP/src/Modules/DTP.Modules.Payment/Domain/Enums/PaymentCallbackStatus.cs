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
        Processed = 2,
        InvalidSignature = 3,
        Failed = 4,
        Ignored = 5
    }
}
