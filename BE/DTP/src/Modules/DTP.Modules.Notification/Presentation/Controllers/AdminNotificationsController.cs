using DTP.Modules.Notification.Application.Commands.Notification;
using DTP.Modules.Notification.Application.DTOs;
using DTP.Modules.Notification.Application.Queries.Notifications;
using DTP.Modules.Notification.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/notifications")]
    public class AdminNotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminNotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] Guid? userId,
            [FromQuery] NotificationType? type,
            [FromQuery] NotificationChannel? channel,
            [FromQuery] NotificationStatus? status,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetAdminNotificationsQuery
                {
                    UserId = userId,
                    Type = type,
                    Channel = channel,
                    Status = status,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                },
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send(
            [FromBody] CreateNotificationRequest request,
            CancellationToken cancellationToken = default)
        {
            var id = await _mediator.Send(
                new SendNotificationCommand
                {
                    Request = request
                },
                cancellationToken);

            return Ok(new { id });
        }
    }
}
