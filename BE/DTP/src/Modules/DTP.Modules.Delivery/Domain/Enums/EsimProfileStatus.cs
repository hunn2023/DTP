using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Domain.Enums
{
    public enum EsimProfileStatus
    {
        Available = 1,
        Assigned = 2,
        Delivered = 3,
        Activated = 4,
        Expired = 5,
        Cancelled = 6
    }
}
