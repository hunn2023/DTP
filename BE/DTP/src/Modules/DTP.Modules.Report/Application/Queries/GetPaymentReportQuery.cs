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
    public class GetPaymentReportQuery : IRequest<Result<PaymentReportDto>>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ReportDateGroupType GroupType { get; set; } = ReportDateGroupType.Day;
    }

    public class GetPaymentReportQueryHandler
    : IRequestHandler<GetPaymentReportQuery, Result<PaymentReportDto>>
    {
        private readonly IReportService _service;

        public GetPaymentReportQueryHandler(IReportService service)
        {
            _service = service;
        }

        public async Task<Result<PaymentReportDto>> Handle(
            GetPaymentReportQuery request,
            CancellationToken cancellationToken)
        {
            return await _service.GetPaymentReportAsync(
                request.FromDate,
                request.ToDate,
                request.GroupType,
                cancellationToken);
        }
    }
}
