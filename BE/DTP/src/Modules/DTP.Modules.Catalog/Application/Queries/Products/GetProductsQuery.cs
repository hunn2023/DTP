
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.Products
{
    public class GetProductsQuery : IRequest<Result<PagedResultDto<ProductDto>>>
    {
        public string? Keyword { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? CountryId { get; set; }

        public Guid? CarrierId { get; set; }

        public Guid? ProviderId { get; set; }

        public bool? IsActive { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
    public class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, Result<PagedResultDto<ProductDto>>>
    {
        private readonly IProductService _productService;

        public GetProductsQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result<PagedResultDto<ProductDto>>> Handle(
            GetProductsQuery request,
            CancellationToken cancellationToken)
        {

            return await _productService.GetPagedAsync(
                request.Keyword,
                request.CategoryId,
                request.CountryId,
                request.CarrierId,
                request.IsActive,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
