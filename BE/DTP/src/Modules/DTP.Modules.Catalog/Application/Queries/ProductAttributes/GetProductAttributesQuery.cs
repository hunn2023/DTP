using DTP.Modules.Catalog.Application.Abstractions.Repositories;
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
        private readonly IProductAttributeRepository _repository;

        public GetProductAttributesQueryHandler(
            IProductAttributeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<ProductAttributeDto>>> Handle(
            GetProductAttributesQuery request,
            CancellationToken cancellationToken)
        {
            var attributes = await _repository.GetListAsync(
                request.ProductId,
                cancellationToken);

            var result = attributes.Select(x => new ProductAttributeDto
            {
                Id = x.Id,
                Name = x.Name,
                Value = x.Value,
                SortOrder = x.SortOrder
            }).ToList();

            return Result<List<ProductAttributeDto>>.Success(result);
        }
    }
}
