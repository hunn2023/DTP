using DTP.Modules.Notification.Application.Abstractions.Services;
using DTP.Modules.Notification.Application.Queries.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/my/notifications")]
    public class MyNotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;

        public MyNotificationsController(
            IMediator mediator,
            INotificationService notificationService)
        {
            _mediator = mediator;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            var result = await _mediator.Send(
                new GetMyNotificationsQuery
                {
                    UserId = userId,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                },
                cancellationToken);

            return Ok(result);
        }

        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            var result = await _notificationService.MarkAsReadAsync(
                id,
                userId,
                cancellationToken);

            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub")
                        ?? User.FindFirstValue("userId");

            if (string.IsNullOrWhiteSpace(value))
                throw new UnauthorizedAccessException("UserId not found.");

            return Guid.Parse(value);
        }
    }
}
