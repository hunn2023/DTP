using DTP.Modules.Knowledge.Application.Commands.ReindexKnowledge;
using DTP.Modules.Knowledge.Application.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Knowledge.Presentation.Controllers
{
    [ApiController]
    [Route("api/knowledge")]
    public class KnowledgeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public KnowledgeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("reindex")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reindex(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new ReindexKnowledgeCommand(),
                cancellationToken);

            return Ok(new
            {
                success = true,
                message = "Re-index knowledge thành công.",
                data = result
            });
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(
            [FromQuery] string query,
            [FromQuery] int topK = 5,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new SearchKnowledgeQuery(query, topK),
                cancellationToken);

            return Ok(new
            {
                success = true,
                data = result
            });
        }
    }
}
