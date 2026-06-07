using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Domain.Enums
{
    public enum AuditActionType
    {
        Unknown = 0,

        Create = 1,
        Update = 2,
        Delete = 3,
        View = 4,
        Login = 5,
        Logout = 6,

        ChangeStatus = 10,
        UploadFile = 11,
        DownloadFile = 12,

        Payment = 20,
        Refund = 21,

        ProviderRequest = 30,
        ProviderResponse = 31,

        System = 100
    }
}
}
