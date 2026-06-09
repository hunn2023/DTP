
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
        //private readonly IProductRepository _productRepository;
        //private readonly IProductImageRepository _productImageRepository;
        //private readonly IFileStorageService _fileStorageService;
        //private readonly IUnitOfWork _unitOfWork;

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
                //request.ProductId,
                request.ImageId,
                cancellationToken);
            //var product = await _productRepository.GetByIdAsync(
            //    request.ProductId,
            //    cancellationToken);

            //if (product == null)
            //    throw new Exception("Không tìm thấy sản phẩm.");

            //var image = await _productImageRepository.GetByIdAsync(
            //    request.ImageId,
            //    cancellationToken);

            //if (image == null || image.ProductId != request.ProductId)
            //    throw new Exception("Không tìm thấy ảnh sản phẩm.");

            //var imageKey = image.ImageKey;
            //var wasThumbnail = image.IsThumbnail;

            //_productImageRepository.Remove(image);

            //await _productImageRepository.SaveChangesAsync(cancellationToken);

            //if (!string.IsNullOrWhiteSpace(imageKey))
            //{
            //    await _fileStorageService.DeleteAsync(
            //        imageKey,
            //        cancellationToken);
            //}

            //if (wasThumbnail)
            //{
            //    var remainingImages = await _productImageRepository.GetByProductIdAsync(
            //        request.ProductId,
            //        cancellationToken);

            //    var nextThumbnail = remainingImages
            //        .OrderBy(x => x.SortOrder)
            //        .FirstOrDefault();

            //    if (nextThumbnail != null)
            //    {
            //        await _productImageRepository.ClearThumbnailAsync(
            //            request.ProductId,
            //            cancellationToken);

            //       // nextThumbnail.SetThumbnail(true);

            //        //product.SetThumbnail(
            //        //    nextThumbnail.ImageUrl,
            //        //    nextThumbnail.ImageKey);

            //        _productImageRepository.Update(nextThumbnail);
            //        _productRepository.Update(product);

            //        await _productImageRepository.SaveChangesAsync(cancellationToken);
            //    }
            //    else
            //    {
            //        //product.ClearThumbnail();

            //        _productRepository.Update(product);

            //        await _productImageRepository.SaveChangesAsync(cancellationToken);
            //    }
            //}

            //return true;
        }
    }
}
