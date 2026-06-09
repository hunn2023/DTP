using DTP.Modules.Report.Application.Abstractions.Exports;
using DTP.Modules.Report.Application.DTOs;
using DTP.Modules.Report.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Queries
{
    public class ExportReportQuery : IRequest<ReportExportResultDto>
    {
        public ReportMetricType ReportType { get; set; }
        public ReportExportFormat Format { get; set; } = ReportExportFormat.Csv;

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class ExportReportQueryHandler
    : IRequestHandler<ExportReportQuery, ReportExportResultDto>
    {
        private readonly IReportExportService _exportService;

        public ExportReportQueryHandler(IReportExportService exportService)
        {
            _exportService = exportService;
        }

        public async Task<ReportExportResultDto> Handle(
            ExportReportQuery request,
            CancellationToken cancellationToken)
        {
            return await _exportService.ExportAsync(
                request.ReportType,
                request.Format,
                request.FromDate,
                request.ToDate,
                cancellationToken);
        }
    }
}
