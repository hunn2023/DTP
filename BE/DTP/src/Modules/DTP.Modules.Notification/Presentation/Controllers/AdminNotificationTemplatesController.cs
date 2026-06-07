using DTP.Modules.Notification.Application.Commands.Template;
using DTP.Modules.Notification.Application.DTOs;
using DTP.Modules.Notification.Application.Queries.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/notification-templates")]
    public class AdminNotificationTemplatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminNotificationTemplatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetNotificationTemplatesQuery(),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTemplateRequest request,
            CancellationToken cancellationToken = default)
        {
            var id = await _mediator.Send(
                new CreateNotificationTemplateCommand
                {
                    Request = request
                },
                cancellationToken);

            return Ok(new { id });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateTemplateRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new UpdateNotificationTemplateCommand
                {
                    Id = id,
                    Request = request
                },
                cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new DeleteNotificationTemplateCommand
                {
                    Id = id
                },
                cancellationToken);

            return Ok(result);
        }
    }
}
