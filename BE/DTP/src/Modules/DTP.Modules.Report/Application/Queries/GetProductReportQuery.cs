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
    public class GetProductReportQuery : IRequest<ProductReportDto>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetProductReportQueryHandler
    : IRequestHandler<GetProductReportQuery, ProductReportDto>
    {
        private readonly IReportService _service;

        public GetProductReportQueryHandler(IReportService service)
        {
            _service = service;
        }

        public async Task<ProductReportDto> Handle(
            GetProductReportQuery request,
            CancellationToken cancellationToken)
        {
            return await _service.GetProductReportAsync(
                request.FromDate,
                request.ToDate,
                cancellationToken);
        }
    }
}
