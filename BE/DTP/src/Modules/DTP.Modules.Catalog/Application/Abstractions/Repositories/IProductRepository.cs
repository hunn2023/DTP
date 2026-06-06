using DTP.Modules.Catalog.Application.Commands.Products;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Persistence;


namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Task<PagedResultDto<ProductDto>> GetPublicPagedAsync(
            string? keyword,
            Guid? categoryId,
            Guid? countryId,
            Guid? carrierId,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<ProductDto?> GetPublicBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<ProductDto>> GetPagedAsync(
            string? keyword,
            Guid? categoryId,
            Guid? countryId,
            Guid? carrierId,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<ProductDto?> GetByIdDtoAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(
            CreateProductCommand command,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            UpdateProductCommand command,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
