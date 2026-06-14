using DTP.Modules.Report.Application.Abstractions.Services;
using DTP.Modules.Report.Application.DTOs;
using DTP.Modules.Report.Domain.Enums;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Queries
{
    public class GetCustomerReportQuery : IRequest<Result<CustomerReportDto>>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ReportDateGroupType GroupType { get; set; } = ReportDateGroupType.Day;
    }

    public class GetCustomerReportQueryHandler
    : IRequestHandler<GetCustomerReportQuery, Result<CustomerReportDto>>
    {
        private readonly IReportService _service;

        public GetCustomerReportQueryHandler(IReportService service)
        {
            _service = service;
        }

        public async Task<Result<CustomerReportDto>> Handle(
            GetCustomerReportQuery request,
            CancellationToken cancellationToken)
        {
            return await _service.GetCustomerReportAsync(
                request.FromDate,
                request.ToDate,
                request.GroupType,
                cancellationToken);
        }
    }
}
