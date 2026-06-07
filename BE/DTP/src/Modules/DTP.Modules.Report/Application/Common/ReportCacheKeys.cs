using DTP.Modules.Report.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Common
{
    public static class ReportCacheKeys
    {
        public static string Dashboard(DateTime from, DateTime to)
            => $"report:dashboard:{from:yyyyMMdd}:{to:yyyyMMdd}";

        public static string Sales(DateTime from, DateTime to, ReportDateGroupType groupType)
            => $"report:sales:{from:yyyyMMdd}:{to:yyyyMMdd}:{groupType}";

        public static string Orders(DateTime from, DateTime to, ReportDateGroupType groupType)
            => $"report:orders:{from:yyyyMMdd}:{to:yyyyMMdd}:{groupType}";

        public static string Payments(DateTime from, DateTime to, ReportDateGroupType groupType)
            => $"report:payments:{from:yyyyMMdd}:{to:yyyyMMdd}:{groupType}";

        public static string Products(DateTime from, DateTime to)
            => $"report:products:{from:yyyyMMdd}:{to:yyyyMMdd}";

        public static string Customers(DateTime from, DateTime to, ReportDateGroupType groupType)
            => $"report:customers:{from:yyyyMMdd}:{to:yyyyMMdd}:{groupType}";

        public static string Providers(DateTime from, DateTime to)
            => $"report:providers:{from:yyyyMMdd}:{to:yyyyMMdd}";

        public static string All => "report:*";
    }
}
