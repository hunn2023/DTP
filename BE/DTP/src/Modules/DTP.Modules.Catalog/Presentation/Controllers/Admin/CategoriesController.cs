using DTP.Modules.Catalog.Application.Commands.Category;
using DTP.Modules.Catalog.Application.Queries.Category;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Presentation.Controllers.Admin
{
    [ApiController]
    //[Authorize(Roles = "Admin")]
    [Route("api/admin/catalog/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Admin categories list
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] GetCategoriesQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Category detail
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(
                new GetCategoryByIdQuery(id));

            return Ok(result);
        }

        /// <summary>
        /// Create category
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCategoryCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Update category
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateCategoryCommand command)
        {
            command.Id = id;

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Delete category
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(
                new DeleteCategoryCommand(id));

            return Ok(result);
        }
    }
}
