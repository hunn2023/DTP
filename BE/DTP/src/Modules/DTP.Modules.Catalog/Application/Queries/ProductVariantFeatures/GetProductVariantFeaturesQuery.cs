using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.ProductVariantFeatures
{
    public class GetProductVariantFeaturesQuery
        : IRequest<Result<List<ProductVariantFeatureDto>>>
    {
        public Guid ProductVariantId { get; set; }

        public GetProductVariantFeaturesQuery(Guid productVariantId)
        {
            ProductVariantId = productVariantId;
        }
    }

    public class GetProductVariantFeaturesQueryHandler
       : IRequestHandler<GetProductVariantFeaturesQuery, Result<List<ProductVariantFeatureDto>>>
    {
        private readonly IProductVariantFeatureService _service;

        public GetProductVariantFeaturesQueryHandler(
            IProductVariantFeatureService service)
        {
            _service = service;
        }

        public async Task<Result<List<ProductVariantFeatureDto>>> Handle(
            GetProductVariantFeaturesQuery request,
            CancellationToken cancellationToken)
        {
            return await _service.GetByProductVariantIdAsync(
                request.ProductVariantId,
                cancellationToken);
        }
    }
}
