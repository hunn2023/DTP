using DTP.Modules.Catalog.Application.Commands.ProductContents;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Application.Queries.ProductContents;
using DTP.Modules.Catalog.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/product-contents")]
    public class AdminProductContentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminProductContentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProductContentByIdQuery(id),
                cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = "Nội dung sản phẩm không tồn tại."
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
                new GetProductContentsByProductIdQuery(
                    productId,
                    onlyActive),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("by-product/{productId:guid}/type/{contentType}")]
        public async Task<IActionResult> GetByProductIdAndType(
            Guid productId,
            ProductContentType contentType,
            [FromQuery] bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetProductContentsByProductIdAndTypeQuery(
                    productId,
                    contentType,
                    onlyActive),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductContentRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateProductContentCommand
            {
                ProductId = request.ProductId,
                ContentType = request.ContentType,
                Title = request.Title,
                Summary = request.Summary,
                BodyHtml = request.BodyHtml,
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
            [FromBody] UpdateProductContentRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateProductContentCommand
            {
                Id = id,
                ContentType = request.ContentType,
                Title = request.Title,
                Summary = request.Summary,
                BodyHtml = request.BodyHtml,
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
                new DeleteProductContentCommand(id),
                cancellationToken);

            return NoContent();
        }
    }
}
