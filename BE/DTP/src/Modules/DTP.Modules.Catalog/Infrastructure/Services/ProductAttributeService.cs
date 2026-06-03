using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Guid> CreateAsync(
            Guid productId,
            string name,
            string value,
            int sortOrder,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
                throw new Exception("ProductId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Vui lòng nhập tên thuộc tính.");

            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Vui lòng nhập giá trị thuộc tính.");

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
            return attribute.Id;
        }

        public async Task UpdateAsync(
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
                throw new Exception("Không tìm thấy thuộc tính sản phẩm.");

            attribute.Update(
                name,
                value,
                sortOrder);

            await _repository.SaveChangesAsync(
                cancellationToken);

            await _cacheInvalidator.ClearProductDetailAsync(attribute.ProductId, cancellationToken);
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var attribute = await _repository.GetByIdAsync(
                id,
                cancellationToken);

            if (attribute == null)
                throw new Exception("Không tìm thấy thuộc tính sản phẩm.");

            _repository.Remove(attribute);

            await _repository.SaveChangesAsync(
                cancellationToken);

            await _cacheInvalidator.ClearProductDetailAsync(attribute.ProductId, cancellationToken);

        }
    }
}
