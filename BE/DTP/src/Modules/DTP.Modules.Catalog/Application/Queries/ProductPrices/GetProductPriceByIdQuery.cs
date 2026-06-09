using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.ProductPrices
{
    public class GetProductPriceByIdQuery : IRequest<Result<ProductPriceDto?>>
    {
        public Guid Id { get; set; }

        public GetProductPriceByIdQuery(Guid id)
        {
            Id = id;
        }
    }


    public class GetProductPriceByIdQueryHandler : IRequestHandler<GetProductPriceByIdQuery, Result<ProductPriceDto?>>
    {
        private readonly IProductPriceRepository _repository;

        public GetProductPriceByIdQueryHandler(IProductPriceRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<ProductPriceDto?>> Handle(GetProductPriceByIdQuery request, CancellationToken cancellationToken)
        {
            var price = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (price == null)
                return null;

            var result = new ProductPriceDto
            {
                Id = price.Id,
                ProductId = price.ProductId,
                ProductVariantId = price.ProductVariantId,
                Currency = price.Currency,
                OriginalPrice = price.OriginalPrice,
                SalePrice = price.SalePrice,
                CostPrice = price.CostPrice,
                StartDate = price.StartDate,
                EndDate = price.EndDate,
                IsActive = price.IsActive
            };

            return Result<ProductPriceDto?>.Success(result);
        }
    }
}
