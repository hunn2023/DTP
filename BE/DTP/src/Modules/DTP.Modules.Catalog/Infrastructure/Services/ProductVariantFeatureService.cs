using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application;


namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProductVariantFeatureService : IProductVariantFeatureService
    {
        private readonly IProductVariantFeatureRepository _repository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly IProductCacheInvalidator _productCacheInvalidator;

        public ProductVariantFeatureService(
            IProductVariantFeatureRepository repository,
            IProductVariantRepository productVariantRepository,
            ICatalogUnitOfWork unitOfWork,
            IProductCacheInvalidator productCacheInvalidator)
        {
            _repository = repository;
            _productVariantRepository = productVariantRepository;
            _unitOfWork = unitOfWork;
            _productCacheInvalidator = productCacheInvalidator;
        }

        public async Task<Result<List<ProductVariantFeatureDto>>> GetByProductVariantIdAsync(
            Guid productVariantId,
            CancellationToken cancellationToken = default)
        {
            if (productVariantId == Guid.Empty)
                return Result<List<ProductVariantFeatureDto>>.Failure("ProductVariantId không hợp lệ.");

            var variant = await _productVariantRepository.GetByIdAsync(
                productVariantId,
                cancellationToken);

            if (variant == null)
                return Result<List<ProductVariantFeatureDto>>.Failure("Không tìm thấy biến thể sản phẩm.");

            var features = await _repository.GetByProductVariantIdAsync(
                productVariantId,
                cancellationToken);

            var result = features.Select(x => new ProductVariantFeatureDto
            {
                Id = x.Id,
                ProductVariantId = x.ProductVariantId,
                Text = x.Text,
                Icon = x.Icon,
                SortOrder = x.SortOrder,
                IsActive = x.IsActive
            }).ToList();

            return Result<List<ProductVariantFeatureDto>>.Success(result);
        }

        public async Task<Result<Guid>> CreateAsync(
            Guid productVariantId,
            string text,
            string? icon,
            int? sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            if (productVariantId == Guid.Empty)
                return Result<Guid>.Failure("ProductVariantId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(text))
                return Result<Guid>.Failure("Vui lòng nhập nội dung feature.");

            var variant = await _productVariantRepository.GetByIdAsync(
                productVariantId,
                cancellationToken);

            if (variant == null)
                return Result<Guid>.Failure("Không tìm thấy biến thể sản phẩm.");

            var finalSortOrder = sortOrder.HasValue && sortOrder.Value > 0
                ? sortOrder.Value
                : await _repository.GetNextSortOrderAsync(productVariantId, cancellationToken);

            var feature = new ProductVariantFeature(
                productVariantId,
                text.Trim(),
                string.IsNullOrWhiteSpace(icon) ? null : icon.Trim(),
                finalSortOrder,
                isActive);

            await _repository.AddAsync(feature, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _productCacheInvalidator.ClearProductDetailAsync(
                variant.ProductId,
                cancellationToken);

            return Result<Guid>.Success(feature.Id);
        }

        public async Task<Result<bool>> UpdateAsync(
            Guid id,
            string text,
            string? icon,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result<bool>.Failure("Id không hợp lệ.");

            if (string.IsNullOrWhiteSpace(text))
                return Result<bool>.Failure("Vui lòng nhập nội dung feature.");

            if (sortOrder <= 0)
                return Result<bool>.Failure("SortOrder phải lớn hơn 0.");

            var feature = await _repository.GetByIdAsync(id, cancellationToken);

            if (feature == null)
                return Result<bool>.Failure("Không tìm thấy feature.");

            var variant = await _productVariantRepository.GetByIdAsync(
                feature.ProductVariantId,
                cancellationToken);

            if (variant == null)
                return Result<bool>.Failure("Không tìm thấy biến thể sản phẩm.");

            feature.Update(
                text.Trim(),
                string.IsNullOrWhiteSpace(icon) ? null : icon.Trim(),
                sortOrder,
                isActive);

            _repository.Update(feature);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _productCacheInvalidator.ClearProductDetailAsync(
                variant.ProductId,
                cancellationToken);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result<bool>.Failure("Id không hợp lệ.");

            var feature = await _repository.GetByIdAsync(id, cancellationToken);

            if (feature == null)
                return Result<bool>.Failure("Không tìm thấy feature.");

            var variant = await _productVariantRepository.GetByIdAsync(
                feature.ProductVariantId,
                cancellationToken);

            if (variant == null)
                return Result<bool>.Failure("Không tìm thấy biến thể sản phẩm.");

            _repository.Remove(feature);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _productCacheInvalidator.ClearProductDetailAsync(
                variant.ProductId,
                cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
