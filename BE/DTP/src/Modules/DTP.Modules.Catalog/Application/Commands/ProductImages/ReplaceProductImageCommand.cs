
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DTP.Modules.Catalog.Application.Commands.ProductImages
{
    public class ReplaceProductImageCommand : IRequest<Result>
    {
        public Guid ProductId { get; set; }

        public Guid ImageId { get; set; }

        public IFormFile File { get; set; } = default!;
    }


    public class ReplaceProductImageCommandHandler
    : IRequestHandler<ReplaceProductImageCommand, Result>
    {
        private readonly IProductImageService _productImageService;
        //private readonly IUnitOfWork _unitOfWork;

        public ReplaceProductImageCommandHandler(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        public async Task<Result> Handle(
            ReplaceProductImageCommand request,
            CancellationToken cancellationToken)
        {

            return await _productImageService.ReplaceImageAsync(
                request.ProductId,
                request.ImageId,
                request.File,
                cancellationToken);
        }
    }
}
