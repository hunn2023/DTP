using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.Products
{
    public class GetProductDetailQuery : IRequest<Result<ProductDetailDto?>>
    {
        public Guid Id { get; set; }

        public GetProductDetailQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetProductDetailQueryHandler
    : IRequestHandler<GetProductDetailQuery, Result<ProductDetailDto?>>
    {
        private readonly IProductService _productService;

        public GetProductDetailQueryHandler( IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result<ProductDetailDto?>> Handle(
            GetProductDetailQuery request,
            CancellationToken cancellationToken)
        {
            return await _productService.GetDetailAsync(
             request.Id,
             cancellationToken);
        }
    }
}
