using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Domain.Enums
{
    public enum OrderStatus
    {
        WaitingPayment = 1,
        Paid = 2,
        Processing = 3,
        Delivered = 4,
        Completed = 5,
        Cancelled = 6,
        PaymentFailed = 7,
        Refunded = 8
    }
}
