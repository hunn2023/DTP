using DTP.Modules.Catalog.Application.Commands.ProductImages;
using DTP.Modules.Catalog.Application.Queries.ProductImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Catalog.Presentation.Controllers.Admin
{
    [ApiController]
   // [Authorize(Roles = "Admin")]
    [Route("api/admin/catalog/product-images")]
    public class ProductImagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductImagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-product/{productId:guid}")]
        public async Task<IActionResult> GetList(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProductImagesQuery(productId),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductImageCommand command,
            CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(command, cancellationToken);

            return Ok(id);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateProductImageCommand command,
            CancellationToken cancellationToken)
        {
            command.Id = id;

            var result = await _mediator.Send(
                command,
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("upload")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Upload(
            Guid productId,
            [FromForm] UploadProductImageRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UploadProductImageCommand
            {
                ProductId = productId,
                File = request.File,
                AltText = request.AltText,
                IsThumbnail = request.IsThumbnail
            };

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }


        //[HttpPut("{imageId:guid}")]
        //public async Task<IActionResult> UpdateInfo(
        //    Guid productId,
        //    Guid imageId,
        //    [FromBody] UpdateProductImageInfoRequest request,
        //    CancellationToken cancellationToken)
        //{
        //    var command = new UpdateProductImageInfoCommand
        //    {
        //        ProductId = productId,
        //        ImageId = imageId,
        //        AltText = request.AltText,
        //        SortOrder = request.SortOrder
        //    };

        //    var result = await _mediator.Send(command, cancellationToken);

        //    return Ok(result);
        //}

        [HttpPut("{imageId:guid}/thumbnail")]
        public async Task<IActionResult> SetThumbnail(
            Guid productId,
            Guid imageId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new SetProductThumbnailCommand(productId, imageId),
                cancellationToken);

            return Ok(result);
        }

        [HttpPut("{imageId:guid}/replace")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> ReplaceImage(
            Guid productId,
            Guid imageId,
            [FromForm] ReplaceProductImageRequest request,
            CancellationToken cancellationToken)
        {
            var command = new ReplaceProductImageCommand
            {
                ProductId = productId,
                ImageId = imageId,
                File = request.File
            };

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid productId,
            Guid imageId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new DeleteProductImageCommand(productId, imageId),
                cancellationToken);

            return Ok(result);
        }
    }
}

public class UploadProductImageRequest
{
    public IFormFile File { get; set; } = default!;

    public string? AltText { get; set; }

    public bool IsThumbnail { get; set; }
}

public class UpdateProductImageInfoRequest
{
    public string? AltText { get; set; }

    public int SortOrder { get; set; }
}

public class ReplaceProductImageRequest
{
    public IFormFile File { get; set; } = default!;
}
