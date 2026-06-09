using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.ProductVariants
{
    public class GetProductVariantsQuery : IRequest<Result<List<ProductVariantDto>>>
    {
        public Guid ProductId { get; set; }

        public GetProductVariantsQuery(Guid productId)
        {
            ProductId = productId;
        }
    }

    public class GetProductVariantsQueryHandler
        : IRequestHandler<GetProductVariantsQuery, Result<List<ProductVariantDto>>>
    {
        private readonly IProductVariantService _productVariantService;

        public GetProductVariantsQueryHandler(
            IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        public async Task<Result<List<ProductVariantDto>>> Handle(
            GetProductVariantsQuery request,
            CancellationToken cancellationToken)
        {
            return await _productVariantService.GetByProductIdAsync(
                request.ProductId,
                cancellationToken);
        }
    }
}
