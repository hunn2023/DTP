
using DTP.Modules.Catalog.Application.Commands.Products;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;


namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProductService
    {
        Task<Result<PagedResultDto<ProductDto>>> GetPublicPagedAsync(
          string? keyword,
          Guid? categoryId,
          Guid? countryId,
          Guid? carrierId,
          int pageIndex,
          int pageSize,
          CancellationToken cancellationToken = default);

        Task<Result<ProductDto?>> GetPublicBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default);

        Task<Result<PagedResultDto<ProductDto>>> GetPagedAsync(
            string? keyword,
            Guid? categoryId,
            Guid? countryId,
            Guid? carrierId,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result<ProductDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<Guid>> CreateAsync(
            CreateProductCommand command,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            UpdateProductCommand command,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
