using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.Products
{
    public class GetProductVariantQuery : IRequest<Result<PagedResultDto<ProductVariantPublicDto>>>
    {
        public string? Keyword { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? CountryId { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetProductVariantQueryHandler
    : IRequestHandler<GetProductVariantQuery, Result<PagedResultDto<ProductVariantPublicDto>>>
    {
        private readonly IProductService _productService;

        public GetProductVariantQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result<PagedResultDto<ProductVariantPublicDto>>> Handle(
            GetProductVariantQuery request,
            CancellationToken cancellationToken)
        {

            return await _productService.GetPublicVariantPagedAsync(
                request.Keyword,
                request.CategoryId,
                request.CountryId,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
