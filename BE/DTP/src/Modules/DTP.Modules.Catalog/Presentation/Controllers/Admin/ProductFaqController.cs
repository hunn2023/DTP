using DTP.Modules.Catalog.Application.Commands.ProductFaqs;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Application.Queries.ProductFaqs;
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
    [Authorize(Roles = "ADMIN")]
    [Route("api/admin/product-faqs")]
    public class ProductFaqController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductFaqController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProductFaqByIdQuery(id),
                cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = "FAQ sản phẩm không tồn tại."
                });
            }

            return Ok(result);
        }

        [HttpGet("by-product/{productId:guid}")]
        public async Task<IActionResult> GetByProductId(
            Guid productId,
            [FromQuery] bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetProductFaqsByProductIdQuery(
                    productId,
                    onlyActive),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductFaqRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateProductFaqCommand
            {
                ProductId = request.ProductId,
                Question = request.Question,
                Answer = request.Answer,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive
            };

            var result = await _mediator.Send(
                command,
                cancellationToken);

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateProductFaqRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateProductFaqCommand
            {
                Id = id,
                Question = request.Question,
                Answer = request.Answer,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive
            };

            var result = await _mediator.Send(
                command,
                cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new DeleteProductFaqCommand(id),
                cancellationToken);

            return NoContent();
        }
    }
}
