using DTP.Modules.Report.Application.DTOs;
using DTP.Modules.Report.Domain.Enums;
using DTP.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Abstractions.Exports
{
    public interface IReportExportService
    {
        Task<Result<ReportExportResultDto>> ExportAsync(
            ReportMetricType reportType,
            ReportExportFormat format,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default);
    }
}
