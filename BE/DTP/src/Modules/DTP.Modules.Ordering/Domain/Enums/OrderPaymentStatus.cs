using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Domain.Enums
{
    public enum OrderPaymentStatus
    {
        Unpaid = 1,
        Paid = 2,
        Failed = 3,
        Refunded = 4
    }
}
