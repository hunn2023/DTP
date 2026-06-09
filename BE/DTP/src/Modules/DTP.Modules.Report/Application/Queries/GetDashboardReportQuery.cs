using DTP.Modules.Report.Application.Abstractions.Services;
using DTP.Modules.Report.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Queries
{
    public class GetDashboardReportQuery : IRequest<DashboardReportDto>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetDashboardReportQueryHandler
    : IRequestHandler<GetDashboardReportQuery, DashboardReportDto>
    {
        private readonly IReportService _service;

        public GetDashboardReportQueryHandler(IReportService service)
        {
            _service = service;
        }

        public async Task<DashboardReportDto> Handle(
            GetDashboardReportQuery request,
            CancellationToken cancellationToken)
        {
            return await _service.GetDashboardReportAsync(
                request.FromDate,
                request.ToDate,
                cancellationToken);
        }
    }
}
