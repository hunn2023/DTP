using DTP.Modules.Report.Application.Abstractions.Services;
using DTP.Modules.Report.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Report.Application.Queries
{
    public class GetProductReportQuery : IRequest<Result<ProductReportDto>>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetProductReportQueryHandler
    : IRequestHandler<GetProductReportQuery, Result<ProductReportDto>>
    {
        private readonly IReportService _service;

        public GetProductReportQueryHandler(IReportService service)
        {
            _service = service;
        }

        public async Task<Result<ProductReportDto>> Handle(
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
