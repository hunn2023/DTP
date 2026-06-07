
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DTP.Modules.Catalog.Application.Commands.ProductImages
{
    public class UploadProductImageCommand : IRequest<Result<ProductImageDto>>
    {
        public Guid ProductId { get; set; }

        public IFormFile File { get; set; } = default!;

        public string? AltText { get; set; }

        public bool IsThumbnail { get; set; }
    }


    public class UploadProductImageCommandHandler
    : IRequestHandler<UploadProductImageCommand, Result<ProductImageDto>>
    {
        private readonly IProductImageService _productImageService;

        public UploadProductImageCommandHandler(
            IProductImageService productImageService
            )
        {
            _productImageService = productImageService;
        }

        public async Task<Result<ProductImageDto>> Handle(
            UploadProductImageCommand request,
            CancellationToken cancellationToken)
        {
           return await _productImageService.UploadAsync(
                request.ProductId,
                request.File,
                request.AltText,
                request.IsThumbnail,
                cancellationToken);
        }
    }
}
