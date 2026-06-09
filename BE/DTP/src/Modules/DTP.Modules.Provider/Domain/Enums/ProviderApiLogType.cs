using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Enums
{
    public enum ProviderApiLogType
    {
        CreateOrder = 1,
        GetOrderStatus = 2,
        GetPackage = 3,
        Webhook = 4,
        Balance = 5
    }
}
