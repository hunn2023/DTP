using DTP.Modules.Report.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Common
{
    public static class ReportPeriodHelper
    {
        public static string GetLabel(DateTime date, ReportDateGroupType groupType)
        {
            return groupType switch
            {
                ReportDateGroupType.Day => date.ToString("yyyy-MM-dd"),
                ReportDateGroupType.Week => $"{date.Year}-W{GetWeekOfYear(date)}",
                ReportDateGroupType.Month => date.ToString("yyyy-MM"),
                ReportDateGroupType.Year => date.ToString("yyyy"),
                _ => date.ToString("yyyy-MM-dd")
            };
        }

        public static DateTime GetGroupDate(DateTime date, ReportDateGroupType groupType)
        {
            return groupType switch
            {
                ReportDateGroupType.Day => date.Date,
                ReportDateGroupType.Week => date.Date.AddDays(-(int)date.DayOfWeek),
                ReportDateGroupType.Month => new DateTime(date.Year, date.Month, 1),
                ReportDateGroupType.Year => new DateTime(date.Year, 1, 1),
                _ => date.Date
            };
        }

        private static int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            return culture.Calendar.GetWeekOfYear(
                date,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }
    }
}
