using System.Security.Claims;
using DTP.Modules.Customer.Application.Features.Admin.Customers.Commands;
using DTP.Modules.Customer.Application.Features.Admin.Customers.Queries;
using DTP.Modules.Customer.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Customer.Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/customer/admin/customers")]
    [Authorize(Roles = "ADMIN")]
    public class AdminCustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminCustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? keyword,
            [FromQuery] bool? isActive,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAdminCustomersQuery
            {
                Keyword = keyword,
                IsActive = isActive,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetDetail(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAdminCustomerDetailQuery
            {
                UserId = userId
            }, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("{userId:guid}/lock")]
        public async Task<IActionResult> Lock(
            Guid userId,
            [FromBody] UpdateCustomerStatusRequestDto? request,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == Guid.Empty)
                return new UnauthorizedObjectResult(new
                {
                    success = false,
                    message = "Không xác định được admin hiện tại."
                });

            var result = await _mediator.Send(new UpdateCustomerStatusCommand
            {
                UserId = userId,
                IsActive = false,
                Reason = request?.Reason,
                UpdatedByUserId = Guid.Empty, // Replace with actual user ID if needed
                IpAddress = GetClientIp(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            }, cancellationToken);          

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{userId:guid}/unlock")]
        public async Task<IActionResult> Unlock(
            Guid userId,
            [FromBody] UpdateCustomerStatusRequestDto? request,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == Guid.Empty)
                return new UnauthorizedObjectResult(new
                {
                    success = false,
                    message = "Không xác định được admin hiện tại."
                });

            var result = await _mediator.Send(new UpdateCustomerStatusCommand
            {
                UserId = userId,
                IsActive = true,
                Reason = request?.Reason,
                UpdatedByUserId = Guid.Empty, // Replace with actual user ID if needed
                IpAddress = GetClientIp(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            }, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)
                ?? User.FindFirst("sub")
                ?? User.FindFirst("userId")
                ?? User.FindFirst("UserId");

            return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var id)
                ? id
                : Guid.Empty;
        }

        private string? GetClientIp()
        {
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(forwardedFor))
                return forwardedFor.Split(',')[0].Trim();

            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
