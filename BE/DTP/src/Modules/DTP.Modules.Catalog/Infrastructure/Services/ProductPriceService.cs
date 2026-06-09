using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application;


namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProductPriceService : IProductPriceService
    {
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly IProductCacheInvalidator _productCacheInvalidator;

        public ProductPriceService(
             IProductPriceRepository productPriceRepository,
             IProductRepository productRepository,
             IProductVariantRepository productVariantRepository,
             ICatalogUnitOfWork unitOfWork,
                IProductCacheInvalidator productCacheInvalidator
             )
        {
            _productPriceRepository = productPriceRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _unitOfWork = unitOfWork;
            _productCacheInvalidator = productCacheInvalidator;
        }

        public async Task<Result<Guid>> CreateAsync(
            Guid productId,
            Guid? productVariantId,
            string currency,
            decimal originalPrice,
            decimal salePrice,
            decimal costPrice,
            DateTime? startDate,
            DateTime? endDate,
            string note,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
                return Result<Guid>.Failure("ProductId is required.");

            currency = currency?.Trim().ToUpper() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(currency))
                return Result<Guid>.Failure("Currency is required.");

            if (originalPrice < 0 || salePrice < 0 || costPrice < 0)
                return Result<Guid>.Failure("Price must be greater than or equal to 0.");

            if (salePrice > originalPrice)
                return Result<Guid>.Failure("SalePrice must be less than or equal to OriginalPrice.");

            if (endDate.HasValue && startDate.HasValue && endDate < startDate)
                return Result<Guid>.Failure("EndDate must be greater than StartDate.");

            var product = await _productRepository.GetByIdAsync(
                productId,
                cancellationToken);

            if (product == null)
                return Result<Guid>.Failure("Product does not exist.");

            if (productVariantId.HasValue && productVariantId.Value != Guid.Empty)
            {
                var variant = await _productVariantRepository.GetByIdAsync(
                    productVariantId.Value,
                    cancellationToken);

                if (variant == null)
                    return Result<Guid>.Failure("Product variant does not exist.");

                if (variant.ProductId != productId)
                    return Result<Guid>.Failure("Product variant does not belong to this product.");
            }
            else
            {
                productVariantId = null;
            }

            var existsActivePrice = await _productPriceRepository.ExistsActivePriceAsync(
                productId,
                productVariantId,
                currency,
                null,
                cancellationToken);

            if (existsActivePrice)
                return Result<Guid>.Failure("Active price already exists for this product or variant.");

            var price = new ProductPrice(
                productId,
                productVariantId,
                currency,
                originalPrice,
                salePrice,
                costPrice,
                startDate,
                endDate,
                note);

            await _productPriceRepository.AddAsync(
                price,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _productCacheInvalidator.ClearProductDetailAsync(
                productId,
                cancellationToken);

            return Result<Guid>.Success(price.Id);
        }

        public async Task<Result> UpdateAsync(
            Guid id,
            string currency,
            decimal originalPrice,
            decimal salePrice,
            decimal costPrice,
            DateTime? startDate,
            DateTime? endDate,
            bool isActive,
            string note,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result.Failure("ProductPriceId is required.");

            var price = await _productPriceRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (price == null)
                return Result.Failure("Product price not found.");

            currency = currency?.Trim().ToUpper() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(currency))
                return Result.Failure("Currency is required.");

            if (originalPrice < 0 || salePrice < 0 || costPrice < 0)
                return Result.Failure("Price must be greater than or equal to 0.");

            if (salePrice > originalPrice)
                return Result.Failure("SalePrice must be less than or equal to OriginalPrice.");

            if (endDate.HasValue && startDate.HasValue && endDate < startDate)
                return Result.Failure("EndDate must be greater than StartDate.");

            if (isActive)
            {
                var exists = await _productPriceRepository.ExistsActivePriceAsync(
                    price.ProductId,
                    price.ProductVariantId,
                    currency,
                    price.Id,
                    cancellationToken);

                if (exists)
                    return Result.Failure("Another active price already exists for this product or variant.");
            }

            price.Update(
                currency,
                originalPrice,
                salePrice,
                costPrice,
                startDate,
                endDate,
                isActive,
                note);

            _productPriceRepository.Update(price);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _productCacheInvalidator.ClearProductDetailAsync(
                price.ProductId,
                cancellationToken);

            return Result.Success();
        }


        public async Task<Result> DeleteProductPriceAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var price = await _productPriceRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (price == null)
                return Result.Failure("Product price not found.");

            price.Deactivate();

            _productPriceRepository.Update(price);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _productCacheInvalidator.ClearProductDetailAsync(
                price.ProductId,
                cancellationToken);

            return Result.Success();
        }

       
    }
}
