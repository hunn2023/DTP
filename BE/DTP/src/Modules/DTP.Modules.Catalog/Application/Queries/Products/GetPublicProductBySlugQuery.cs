using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.Products
{
    public class GetPublicProductBySlugQuery
       : IRequest<Result<ProductDto?>>
    {
        public string Slug { get; set; }

        public GetPublicProductBySlugQuery(string slug)
        {
            Slug = slug;
        }
    }

    public class GetPublicProductBySlugQueryHandler
       : IRequestHandler<GetPublicProductBySlugQuery, Result<ProductDto?>>
    {
        private readonly IProductService _productService;

        public GetPublicProductBySlugQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result<ProductDto?>> Handle(
            GetPublicProductBySlugQuery request,
            CancellationToken cancellationToken)
        {
            return await _productService.GetPublicBySlugAsync(
                request.Slug,
                cancellationToken);
        }
    }
}
