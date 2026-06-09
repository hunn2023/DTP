using DTP.Modules.Report.Application.Abstractions.Services;
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
    public class GetSalesReportQuery : IRequest<SalesReportDto>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ReportDateGroupType GroupType { get; set; } = ReportDateGroupType.Day;
    }

    public class GetSalesReportQueryHandler
    : IRequestHandler<GetSalesReportQuery, SalesReportDto>
    {
        private readonly IReportService _service;

        public GetSalesReportQueryHandler(IReportService service)
        {
            _service = service;
        }

        public async Task<SalesReportDto> Handle(
            GetSalesReportQuery request,
            CancellationToken cancellationToken)
        {
            return await _service.GetSalesReportAsync(
                request.FromDate,
                request.ToDate,
                request.GroupType,
                cancellationToken);
        }
    }
}
