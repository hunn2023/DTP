using Azure.Core;
using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProductAttributeService : IProductAttributeService
    {
        private readonly IProductAttributeRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IProductCacheInvalidator _cacheInvalidator;

        public ProductAttributeService(
            IProductAttributeRepository repository,
            IProductRepository productRepository,
            IProductCacheInvalidator cacheInvalidator)
        {
            _repository = repository;
            _productRepository = productRepository;
            _cacheInvalidator = cacheInvalidator;
        }


        public async Task<Result<List<ProductAttributeDto>>> GetListAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var attributes = await _repository.GetListAsync(
                productId,
                cancellationToken);

            var result = attributes.Select(x => new ProductAttributeDto
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value,
                SortOrder = x.SortOrder
            }).ToList();

            return Result<List<ProductAttributeDto>>.Success(result);
        }

        public async Task<Result<Guid>> CreateAsync(
            Guid productId,
            string key,
            string? displayName,
            string value,
            int sortOrder,
            bool isVisible,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
                return Result<Guid>.Failure("ProductId không hợp lệ.");

            var productExists = await _productRepository.ExistsAsync(
                productId,
                cancellationToken);

            if (!productExists)
                return Result<Guid>.Failure("Không tìm thấy sản phẩm.");

            if (string.IsNullOrWhiteSpace(key))
                return Result<Guid>.Failure("Vui lòng nhập mã thuộc tính.");

            if (string.IsNullOrWhiteSpace(value))
                return Result<Guid>.Failure("Vui lòng nhập giá trị thuộc tính.");

            key = key.Trim();
            displayName = string.IsNullOrWhiteSpace(displayName)
                ? null
                : displayName.Trim();

            value = value.Trim();

            var attribute = new ProductAttribute(
                productId,
                key,
                displayName,
                value,
                sortOrder,
                isVisible);

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
            string key,
            string? displayName,
            string value,
            int sortOrder,
            bool isVisible,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result.Failure("Id không hợp lệ.");

            if (string.IsNullOrWhiteSpace(key))
                return Result.Failure("Vui lòng nhập mã thuộc tính.");

            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure("Vui lòng nhập giá trị thuộc tính.");

            var attribute = await _repository.GetByIdAsync(
                id,
                cancellationToken);

            if (attribute == null)
                return Result.Failure("Không tìm thấy thuộc tính sản phẩm.");

            key = key.Trim();
            displayName = string.IsNullOrWhiteSpace(displayName)
                ? null
                : displayName.Trim();

            value = value.Trim();

            attribute.Update(
                key,
                displayName,
                value,
                sortOrder,
                isVisible);

            await _repository.SaveChangesAsync(
                cancellationToken);

            await _cacheInvalidator.ClearProductDetailAsync(
                attribute.ProductId,
                cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result.Failure("Id không hợp lệ.");

            var attribute = await _repository.GetByIdAsync(
                id,
                cancellationToken);

            if (attribute == null)
                return Result.Failure("Không tìm thấy thuộc tính sản phẩm.");

            _repository.Remove(attribute);

            await _repository.SaveChangesAsync(
                cancellationToken);

            await _cacheInvalidator.ClearProductDetailAsync(
                attribute.ProductId,
                cancellationToken);

            return Result.Success();
        }

    }
}