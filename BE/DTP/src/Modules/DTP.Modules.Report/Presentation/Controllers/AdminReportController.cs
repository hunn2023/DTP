using DTP.Modules.Report.Application.Queries;
using DTP.Modules.Report.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/reports")]
    [Authorize]
    public class AdminReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetDashboardReportQuery
            {
                FromDate = fromDate,
                ToDate = toDate
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSales(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] ReportDateGroupType groupType = ReportDateGroupType.Day,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetSalesReportQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                GroupType = groupType
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] ReportDateGroupType groupType = ReportDateGroupType.Day,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetOrderReportQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                GroupType = groupType
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] ReportDateGroupType groupType = ReportDateGroupType.Day,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetPaymentReportQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                GroupType = groupType
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetProductReportQuery
            {
                FromDate = fromDate,
                ToDate = toDate
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] ReportDateGroupType groupType = ReportDateGroupType.Day,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomerReportQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                GroupType = groupType
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("providers")]
        public async Task<IActionResult> GetProviders(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetProviderReportQuery
            {
                FromDate = fromDate,
                ToDate = toDate
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export(
            [FromQuery] ReportMetricType reportType,
            [FromQuery] ReportExportFormat format = ReportExportFormat.Csv,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new ExportReportQuery
            {
                ReportType = reportType,
                Format = format,
                FromDate = fromDate,
                ToDate = toDate
            }, cancellationToken);

            return File(result.Content, result.ContentType, result.FileName);
        }
    }
}