using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Domain.Enums
{
    public enum DigitalDeliveryStatus
    {
        Pending = 1,
        Delivered = 2,
        Failed = 3,
        Retrying = 4
    }
}
