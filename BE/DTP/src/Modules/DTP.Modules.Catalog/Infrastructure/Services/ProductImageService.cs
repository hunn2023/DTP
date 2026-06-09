using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
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

        public ProductImageService(
            IProductImageRepository repository,
            IFileStorageService fileStorageService,
            IProductRepository productRepository)
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

            var productExists = await _productRepository.ExistsAsync(productId, cancellationToken);

            if (!productExists)
                return Result<Guid>.Failure("Không tìm thấy sản phẩm.");

            if (isThumbnail)
            {
                await _repository.ClearThumbnailAsync(productId, cancellationToken);
            }

            var image = new ProductImage(
                productId: productId,
                imageUrl: imageUrl,
                imageKey: string.Empty,
                altText: altText,
                sortOrder: sortOrder,
                isThumbnail: isThumbnail,
                contentType: null,
                size: 0,
                isActive: true);

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
            if (id == Guid.Empty)
                return Result.Failure("Id không hợp lệ.");

            if (string.IsNullOrWhiteSpace(imageUrl))
                return Result.Failure("Vui lòng nhập đường dẫn ảnh.");

            var image = await _repository.GetByIdAsync(id, cancellationToken);

            if (image == null)
                return Result.Failure("Không tìm thấy ảnh sản phẩm.");

            if (isThumbnail && !image.IsThumbnail)
            {
                await _repository.ClearThumbnailAsync(image.ProductId, cancellationToken);
            }

            image.Update(
                imageUrl: imageUrl,
                imageKey: image.ImageKey,
                altText: altText,
                sortOrder: sortOrder,
                isThumbnail: isThumbnail,
                contentType: image.ContentType,
                size: image.Size,
                isActive: image.IsActive);

            _repository.Update(image);
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
            if (productId == Guid.Empty)
                return Result<ProductImageDto>.Failure("ProductId không hợp lệ.");

            if (file == null || file.Length <= 0)
                return Result<ProductImageDto>.Failure("Vui lòng chọn ảnh cần upload.");

            var productExists = await _productRepository.ExistsAsync(productId, cancellationToken);

            if (!productExists)
                return Result<ProductImageDto>.Failure("Không tìm thấy sản phẩm.");

            var sortOrder = await _repository.GetNextSortOrderAsync(
                productId,
                cancellationToken);

            var existingImages = await _repository.GetByProductIdAsync(
                productId,
                cancellationToken);

            var hasImages = existingImages.Any();
            var shouldBeThumbnail = isThumbnail || !hasImages;

            var uploadResult = await _fileStorageService.UploadImageAsync(
                file,
                UploadFolders.ProductImages,
                cancellationToken);

            if (shouldBeThumbnail)
            {
                await _repository.ClearThumbnailAsync(productId, cancellationToken);
            }

            var image = new ProductImage(
                productId: productId,
                imageUrl: uploadResult.Url,
                imageKey: uploadResult.Key,
                altText: altText,
                sortOrder: sortOrder,
                isThumbnail: shouldBeThumbnail,
                contentType: file.ContentType,
                size: file.Length,
                isActive: true);

            await _repository.AddAsync(image, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return Result<ProductImageDto>.Success(MapToDto(image));
        }

        public async Task<Result> SetThumbnailAsync(
            Guid productId,
            Guid imageId,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
                return Result.Failure("ProductId không hợp lệ.");

            if (imageId == Guid.Empty)
                return Result.Failure("ImageId không hợp lệ.");

            var image = await _repository.GetByIdAsync(imageId, cancellationToken);

            if (image == null || image.ProductId != productId)
                return Result.Failure("Không tìm thấy ảnh sản phẩm.");

            if (!image.IsActive)
                return Result.Failure("Ảnh sản phẩm đã bị tắt.");

            await _repository.ClearThumbnailAsync(productId, cancellationToken);

            image.SetThumbnail();

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
            if (productId == Guid.Empty)
                return Result.Failure("ProductId không hợp lệ.");

            if (imageId == Guid.Empty)
                return Result.Failure("ImageId không hợp lệ.");

            var image = await _repository.GetByIdAsync(imageId, cancellationToken);

            if (image == null || image.ProductId != productId)
                return Result.Failure("Không tìm thấy ảnh sản phẩm.");

            image.UpdateInfo(
                altText: altText,
                sortOrder: sortOrder);

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
            if (productId == Guid.Empty)
                return Result.Failure("ProductId không hợp lệ.");

            if (imageId == Guid.Empty)
                return Result.Failure("ImageId không hợp lệ.");

            if (file == null || file.Length <= 0)
                return Result.Failure("Vui lòng chọn ảnh cần thay thế.");

            var image = await _repository.GetByIdAsync(imageId, cancellationToken);

            if (image == null || image.ProductId != productId)
                return Result.Failure("Không tìm thấy ảnh sản phẩm.");

            var oldImageKey = image.ImageKey;

            var uploadResult = await _fileStorageService.UploadImageAsync(
                file,
                UploadFolders.ProductImages,
                cancellationToken);

            image.ReplaceImage(
                  imageUrl: uploadResult.Url,
                  imageKey: uploadResult.Key,
                  contentType: file.ContentType,
                  size: file.Length);

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
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result.Failure("Id không hợp lệ.");

            var image = await _repository.GetByIdAsync(id, cancellationToken);

            if (image == null)
                return Result.Failure("Không tìm thấy ảnh sản phẩm.");

            return await DeleteInternalAsync(image, cancellationToken);
        }

        public async Task<Result> DeleteAsync(
            Guid productId,
            Guid imageId,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
                return Result.Failure("ProductId không hợp lệ.");

            if (imageId == Guid.Empty)
                return Result.Failure("ImageId không hợp lệ.");

            var image = await _repository.GetByIdAsync(imageId, cancellationToken);

            if (image == null || image.ProductId != productId)
                return Result.Failure("Không tìm thấy ảnh sản phẩm.");

            return await DeleteInternalAsync(image, cancellationToken);
        }

        private async Task<Result> DeleteInternalAsync(
            ProductImage image,
            CancellationToken cancellationToken)
        {
            var productId = image.ProductId;
            var imageKey = image.ImageKey;
            var wasThumbnail = image.IsThumbnail;

            _repository.Remove(image);
            await _repository.SaveChangesAsync(cancellationToken);

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
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.SortOrder)
                    .FirstOrDefault();

                if (nextThumbnail != null)
                {
                    await _repository.ClearThumbnailAsync(productId, cancellationToken);

                    nextThumbnail.SetThumbnail();

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