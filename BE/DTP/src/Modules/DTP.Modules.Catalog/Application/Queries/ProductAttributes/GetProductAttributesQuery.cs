using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.ProductAttributes
{

    public class GetProductAttributesQuery : IRequest<Result<List<ProductAttributeDto>>>
    {
        public Guid ProductId { get; }

        public GetProductAttributesQuery(Guid productId)
        {
            ProductId = productId;
        }
    }
    public class GetProductAttributesQueryHandler
    : IRequestHandler<GetProductAttributesQuery, Result<List<ProductAttributeDto>>>
    {
        private readonly IProductAttributeService _productAttributeService;


        public GetProductAttributesQueryHandler(IProductAttributeService productAttributeService)
        {
            _productAttributeService = productAttributeService;
        }

        public async Task<Result<List<ProductAttributeDto>>> Handle(
            GetProductAttributesQuery request,
            CancellationToken cancellationToken)
        {
             return await _productAttributeService.GetListAsync(
                 request.ProductId,
                 cancellationToken);
        }
    }
}
