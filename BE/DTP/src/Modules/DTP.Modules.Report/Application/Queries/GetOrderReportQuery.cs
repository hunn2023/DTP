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
    public class GetOrderReportQuery : IRequest<OrderReportDto>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ReportDateGroupType GroupType { get; set; } = ReportDateGroupType.Day;
    }

    public class GetOrderReportQueryHandler
    : IRequestHandler<GetOrderReportQuery, OrderReportDto>
    {
        private readonly IReportService _service;

        public GetOrderReportQueryHandler(IReportService service)
        {
            _service = service;
        }

        public async Task<OrderReportDto> Handle(
            GetOrderReportQuery request,
            CancellationToken cancellationToken)
        {
            return await _service.GetOrderReportAsync(
                request.FromDate,
                request.ToDate,
                request.GroupType,
                cancellationToken);
        }
    }
}
