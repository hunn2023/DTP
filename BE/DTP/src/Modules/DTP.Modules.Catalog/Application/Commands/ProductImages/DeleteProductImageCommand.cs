
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.ProductImages
{
    public class DeleteProductImageCommand : IRequest<Result>
    {
        public Guid ProductId { get; set; }

        public Guid ImageId { get; set; }

        public DeleteProductImageCommand(Guid productId, Guid imageId)
        {
            ProductId = productId;
            ImageId = imageId;
        }
    }

    public class DeleteProductImageCommandHandler
     : IRequestHandler<DeleteProductImageCommand, Result>
    {

        private readonly IProductImageService _productImageService;

        public DeleteProductImageCommandHandler(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        public async Task<Result> Handle(
            DeleteProductImageCommand request,
            CancellationToken cancellationToken)
        {

            return await _productImageService.DeleteAsync(
                request.ProductId,
                request.ImageId,
                cancellationToken);
        }
    }
}
