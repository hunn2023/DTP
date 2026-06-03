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
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _repository;

        public ProductImageService(IProductImageRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CreateAsync(
            Guid productId,
            string imageUrl,
            string? altText,
            int sortOrder,
            bool isThumbnail,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
                throw new Exception("ProductId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new Exception("Vui lòng nhập đường dẫn ảnh.");

            var image = new ProductImage(
                productId,
                imageUrl,
                altText,
                sortOrder,
                isThumbnail);

            await _repository.AddAsync(image, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return image.Id;
        }

        public async Task UpdateAsync(
            Guid id,
            string imageUrl,
            string? altText,
            int sortOrder,
            bool isThumbnail,
            CancellationToken cancellationToken = default)
        {
            var image = await _repository.GetByIdAsync(id, cancellationToken);

            if (image == null)
                throw new Exception("Không tìm thấy ảnh sản phẩm.");

            image.Update(imageUrl, altText, sortOrder, isThumbnail);

            await _repository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var image = await _repository.GetByIdAsync(id, cancellationToken);

            if (image == null)
                throw new Exception("Không tìm thấy ảnh sản phẩm.");

            _repository.Remove(image);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
