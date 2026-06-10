using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Domain.Enums
{
    public enum DeliveryStatus
    {
        Pending = 1,
        Processing = 2,
        Delivered = 3,
        Failed = 4,
        Cancelled = 5
    }
}
