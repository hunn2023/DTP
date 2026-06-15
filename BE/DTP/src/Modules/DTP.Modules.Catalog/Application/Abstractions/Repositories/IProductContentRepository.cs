using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface IProductContentRepository
    {
        Task<ProductContent?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<ProductContent>> GetByProductIdAsync(
            Guid productId,
            bool onlyActive = false,
            CancellationToken cancellationToken = default);

        Task<List<ProductContent>> GetByProductIdAndTypeAsync(
            Guid productId,
            ProductContentType contentType,
            bool onlyActive = false,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProductContent productContent,
            CancellationToken cancellationToken = default);

        void Update(ProductContent productContent);

        void Delete(ProductContent productContent);
    }
}
