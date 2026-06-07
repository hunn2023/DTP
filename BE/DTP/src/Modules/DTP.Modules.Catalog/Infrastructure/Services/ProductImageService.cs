using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Infrastructure.Repositories;
using DTP.Shared.Application;
using DTP.Shared.Storage;
using Microsoft.AspNetCore.Http;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _repository;
        private readonly IFileStorageService _fileStorageService;
        //private readonly IUnitOfWork _unitOfWork;

        public ProductImageService(IProductImageRepository repository, IFileStorageService fileStorageService, IProductRepository productRepository)
        {
            _repository = repository;
            _fileStorageService = fileStorageService;
            _productRepository = productRepository;
        }

        public async Task<Result<Guid>> CreateAsync(
            Guid productId,
            string imageUrl,
            string? altText,
            int sortOrder,
            bool isThumbnail,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
                return Result<Guid>.Failure("ProductId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(imageUrl))
                return Result<Guid>.Failure("Vui lòng nhập đường dẫn ảnh.");

            var image = new ProductImage(
                productId,
                imageUrl,
                altText,
                sortOrder,
                isThumbnail);

            await _repository.AddAsync(image, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(image.Id);
        }

        public async Task<Result> UpdateAsync(
            Guid id,
            string imageUrl,
            string? altText,
            int sortOrder,
            bool isThumbnail,
            CancellationToken cancellationToken = default)
        {
            var image = await _repository.GetByIdAsync(id, cancellationToken);

            if (image == null)
                return Result.Failure("Không tìm thấy ảnh sản phẩm.");

            //image.Update(imageUrl, altText, sortOrder, isThumbnail);

            await _repository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var image = await _repository.GetByIdAsync(id, cancellationToken);

            if (image == null)
                return Result.Failure("Không tìm thấy ảnh sản phẩm.");

            _repository.Remove(image);
            await _repository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<ProductImageDto>> UploadAsync(
           Guid productId,
           IFormFile file,
           string? altText,
           bool isThumbnail,
           CancellationToken cancellationToken = default)
        {
            var productExists = await _productRepository.ExistsAsync(productId, cancellationToken);

            if (!productExists)
                return Result<ProductImageDto>.Failure("Không tìm thấy sản phẩm.");

            var sortOrder = await _repository.GetNextSortOrderAsync(
                productId,
                cancellationToken);

            var uploadResult = await _fileStorageService.UploadImageAsync(
                file,
                "products",
                cancellationToken);

            var hasImages = (await _repository.GetByProductIdAsync(
                productId,
                cancellationToken)).Any();

            var shouldBeThumbnail = isThumbnail || !hasImages;

            if (shouldBeThumbnail)
            {
                await _repository.ClearThumbnailAsync(
                    productId,
                    cancellationToken);
            }

            var image = new ProductImage(
                productId: productId,
                imageUrl: uploadResult.Url,
                imageKey: uploadResult.Key,
                altText: altText,
                sortOrder: sortOrder,
                isThumbnail: shouldBeThumbnail);

            await _repository.AddAsync(image, cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);

            return Result<ProductImageDto>.Success(MapToDto(image));           
        }


        public async Task<Result> SetThumbnailAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken = default)
        {
            var image = await _repository.GetByIdAsync(
                imageId,
                cancellationToken);

            if (image == null || image.ProductId != productId)
                return Result.Failure("Không tìm thấy ảnh sản phẩm.");

            await _repository.ClearThumbnailAsync(
                productId,
                cancellationToken);

            //image.SetThumbnail(true);

            _repository.Update(image);

            await _repository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> UpdateInfoAsync(
            Guid productId,
            Guid imageId,
            string? altText,
            int sortOrder,
            CancellationToken cancellationToken = default)
        {
            var image = await _repository.GetByIdAsync(
                imageId,
                cancellationToken);

            if (image == null || image.ProductId != productId)
                return Result.Failure("Không tìm thấy ảnh sản phẩm.");

            //image.UpdateInfo(altText, sortOrder);

            _repository.Update(image);

            await _repository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> ReplaceImageAsync(
            Guid productId,
            Guid imageId,
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            var image = await _repository.GetByIdAsync(
                imageId,
                cancellationToken);

            if (image == null || image.ProductId != productId)
                return Result.Failure("Không tìm thấy ảnh sản phẩm.");

            var oldImageKey = image.ImageKey;

            var uploadResult = await _fileStorageService.UploadImageAsync(
                file,
                "products",
                cancellationToken);

            //image.ReplaceImage(
            //    uploadResult.Url,
            //    uploadResult.Key);

            _repository.Update(image);

            await _repository.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(oldImageKey))
            {
                await _fileStorageService.DeleteAsync(
                    oldImageKey,
                    cancellationToken);
            }

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(
            Guid productId,
            Guid imageId,
            CancellationToken cancellationToken = default)
        {
            var image = await _repository.GetByIdAsync(
                imageId,
                cancellationToken);

            if (image == null || image.ProductId != productId)
                return Result.Failure("Không tìm thấy ảnh sản phẩm.");

            var imageKey = image.ImageKey;
            var wasThumbnail = image.IsThumbnail;

            _repository.Remove(image);

            //await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(imageKey))
            {
                await _fileStorageService.DeleteAsync(
                    imageKey,
                    cancellationToken);
            }

            if (wasThumbnail)
            {
                var remainingImages = await _repository.GetByProductIdAsync(
                    productId,
                    cancellationToken);

                var nextThumbnail = remainingImages
                    .OrderBy(x => x.SortOrder)
                    .FirstOrDefault();

                if (nextThumbnail != null)
                {
                    //nextThumbnail.SetThumbnail(true);
                    _repository.Update(nextThumbnail);
                    await _repository.SaveChangesAsync(cancellationToken);
                }
            }

            return Result.Success();
        }

        private static ProductImageDto MapToDto(ProductImage image)
        {
            return new ProductImageDto
            {
                Id = image.Id,
                ProductId = image.ProductId,
                ImageUrl = image.ImageUrl,
                AltText = image.AltText,
                SortOrder = image.SortOrder,
                IsThumbnail = image.IsThumbnail
            };
        }


      
    }
}
