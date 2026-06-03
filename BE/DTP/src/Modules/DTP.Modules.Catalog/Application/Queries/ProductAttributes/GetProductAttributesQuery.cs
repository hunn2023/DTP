using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.ProductAttributes
{

    public class GetProductAttributesQuery : IRequest<List<ProductAttributeDto>>
    {
        public Guid ProductId { get; }

        public GetProductAttributesQuery(Guid productId)
        {
            ProductId = productId;
        }
    }
    public class GetProductAttributesQueryHandler
    : IRequestHandler<GetProductAttributesQuery, List<ProductAttributeDto>>
    {
        private readonly IProductAttributeRepository _repository;

        public GetProductAttributesQueryHandler(
            IProductAttributeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductAttributeDto>> Handle(
            GetProductAttributesQuery request,
            CancellationToken cancellationToken)
        {
            var attributes = await _repository.GetListAsync(
                request.ProductId,
                cancellationToken);

            return attributes.Select(x => new ProductAttributeDto
            {
                Id = x.Id,
                Name = x.Name,
                Value = x.Value,
                SortOrder = x.SortOrder
            }).ToList();
        }
    }

}
