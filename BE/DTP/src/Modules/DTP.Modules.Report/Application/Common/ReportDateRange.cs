using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Common
{
    public class ReportDateRange
    {
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }

        public ReportDateRange(DateTime? fromDate, DateTime? toDate)
        {
            ToDate = toDate?.Date.AddDays(1).AddTicks(-1)
                     ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);

            FromDate = fromDate?.Date
                       ?? DateTime.UtcNow.Date.AddDays(-30);

            if (FromDate > ToDate)
                throw new ArgumentException("FromDate must be less than or equal ToDate.");
        }
    }
}
