using DTP.Modules.Chatbot.Application.Commands;
using DTP.Modules.Chatbot.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Presentation.Controllers
{
    [ApiController]
    [Route("api/public/chatbot")]
    public sealed class PublicChatbotController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicChatbotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("message")]
        public async Task<ActionResult<ChatResponseDto>> SendMessage(
            [FromBody] ChatRequestDto request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(
                new SendChatMessageCommand(request),
                cancellationToken);

            return Ok(response);
        }
    }
}
