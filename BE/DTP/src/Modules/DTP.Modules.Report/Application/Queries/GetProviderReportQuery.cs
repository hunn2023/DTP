using DTP.Modules.Report.Application.Abstractions.Services;
using DTP.Modules.Report.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Queries
{
    public class GetProviderReportQuery : IRequest<Result<ProviderReportDto>>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }


    public class GetProviderReportQueryHandler
    : IRequestHandler<GetProviderReportQuery, Result<ProviderReportDto>>
    {
        private readonly IReportService _service;

        public GetProviderReportQueryHandler(IReportService service)
        {
            _service = service;
        }

        public async Task<Result<ProviderReportDto>> Handle(
            GetProviderReportQuery request,
            CancellationToken cancellationToken)
        {
            return await _service.GetProviderReportAsync(
                request.FromDate,
                request.ToDate,
                cancellationToken);
        }
    }
}
