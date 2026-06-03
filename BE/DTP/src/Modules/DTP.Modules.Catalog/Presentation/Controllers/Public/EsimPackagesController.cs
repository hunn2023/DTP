using DTP.Modules.Catalog.Application.Commands.EsimPackages;
using DTP.Modules.Catalog.Application.Queries.EsimPackages;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/catalog/esim-packages")]
    public class EsimPackagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EsimPackagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] GetPublicEsimPackagesQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var result = await _mediator.Send(
                new GetPublicEsimPackageBySlugQuery(slug));

            return Ok(result);
        }
    }
}
