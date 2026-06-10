using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Domain.Enums
{
    public enum OrderStatus
    {
        Draft = 1,
        PendingPayment = 2,
        Paid = 3,
        Processing = 4,
        Completed = 5,
        Cancelled = 6,
        Failed = 7
    }
}
