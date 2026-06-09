using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;
namespace DTP.Modules.Catalog.Application.Commands.ProductImages
{
    public class SetProductThumbnailCommand : IRequest<Result>
    {
        public Guid ProductId { get; set; }

        public Guid ImageId { get; set; }

        public SetProductThumbnailCommand(Guid productId, Guid imageId)
        {
            ProductId = productId;
            ImageId = imageId;
        }
    }


    public class SetProductThumbnailCommandHandler
    : IRequestHandler<SetProductThumbnailCommand, Result>
    {
        private readonly IProductImageService _productImageService;


        public SetProductThumbnailCommandHandler(
            IProductImageService productImageService
            )
        {
            _productImageService = productImageService;
        }

        public async Task<Result> Handle(
            SetProductThumbnailCommand request,
            CancellationToken cancellationToken)
        {
           return await _productImageService.SetThumbnailAsync(
                request.ProductId,
                request.ImageId,
                cancellationToken);
        }
    }
}
