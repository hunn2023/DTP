using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.ProductPrices
{
    public class GetProductPricesQuery : IRequest<Result<List<ProductPriceDto>>>
    {
        public Guid? ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
    }

    public class GetProductPricesQueryHandler : IRequestHandler<GetProductPricesQuery, Result<List<ProductPriceDto>>>
    {
        private readonly IProductPriceRepository _repository;

        public GetProductPricesQueryHandler(IProductPriceRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<ProductPriceDto>>> Handle(GetProductPricesQuery request, CancellationToken cancellationToken)
        {
            var prices = await _repository.GetListAsync(
                request.ProductId,
                request.ProductVariantId,
                cancellationToken);

            var result = prices.Select(x => new ProductPriceDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductVariantId = x.ProductVariantId,
                Currency = x.Currency,
                OriginalPrice = x.OriginalPrice,
                SalePrice = x.SalePrice,
                CostPrice = x.CostPrice,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive
            }).ToList();

            return Result<List<ProductPriceDto>>.Success(result);
        }
    }
}
