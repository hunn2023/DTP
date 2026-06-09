using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Enums
{
    public enum ProviderOrderStatus
    {
        Pending = 0,
        Processing = 1,
        Success = 2,
        Failed = 3,
        Cancelled = 4,
        WaitingWebhook = 5
    }
}
