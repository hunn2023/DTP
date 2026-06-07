using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application;


namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProductAttributeService : IProductAttributeService
    {
        private readonly IProductAttributeRepository _repository;
        private readonly IProductCacheInvalidator _cacheInvalidator;
        public ProductAttributeService(IProductAttributeRepository repository, IProductCacheInvalidator cacheInvalidator)
        {
            _repository = repository;
            _cacheInvalidator = cacheInvalidator;
        }

        public async Task<Result<Guid>> CreateAsync(
            Guid productId,
            string name,
            string value,
            int sortOrder,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
                return Result<Guid>.Failure("ProductId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(name))
                return Result<Guid>.Failure("Vui lòng nhập tên thuộc tính.");

            if (string.IsNullOrWhiteSpace(value))
                return Result<Guid>.Failure("Vui lòng nhập giá trị thuộc tính.");


            var attribute = new ProductAttribute(
                productId,
                name,
                value,
                sortOrder);

            await _repository.AddAsync(
                attribute,
                cancellationToken);

            await _repository.SaveChangesAsync(
                cancellationToken);

            await _cacheInvalidator.ClearProductDetailAsync(
                productId,
                cancellationToken);
            return Result<Guid>.Success(attribute.Id);
        }

        public async Task<Result> UpdateAsync(
            Guid id,
            string name,
            string value,
            int sortOrder,
            CancellationToken cancellationToken = default)
        {
            var attribute = await _repository.GetByIdAsync(
                id,
                cancellationToken);

            if (attribute == null)
                return Result.Failure("Không tìm thấy thuộc tính sản phẩm.");

            attribute.Update(
                name,
                value,
                sortOrder);

            await _repository.SaveChangesAsync(
                cancellationToken);

            await _cacheInvalidator.ClearProductDetailAsync(attribute.ProductId, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var attribute = await _repository.GetByIdAsync(
                id,
                cancellationToken);

            if (attribute == null)
                return Result.Failure("Không tìm thấy thuộc tính sản phẩm.");

            _repository.Remove(attribute);

            await _repository.SaveChangesAsync(
                cancellationToken);

            await _cacheInvalidator.ClearProductDetailAsync(attribute.ProductId, cancellationToken);

            return Result.Success();
        }
    }
}
