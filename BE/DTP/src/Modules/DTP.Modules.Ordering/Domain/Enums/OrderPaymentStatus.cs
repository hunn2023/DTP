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
        Pending = 2,
        Paid = 3,
        Failed = 4,
        Refunded = 5
    }
}
