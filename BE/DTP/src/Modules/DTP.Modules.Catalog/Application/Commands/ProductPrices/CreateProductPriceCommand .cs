using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Domain.Entities;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductPrices
{
    public class CreateProductPriceCommand : IRequest<Guid>
    {
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public string Currency { get; set; } = "VND";
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CreateProductPriceCommandHandler : IRequestHandler<CreateProductPriceCommand, Guid>
    {
        private readonly IProductPriceRepository _repository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly IProductCacheInvalidator _productCacheInvalidator;
        public CreateProductPriceCommandHandler(
            IProductPriceRepository repository,
            ICatalogUnitOfWork unitOfWork,
            IProductCacheInvalidator productCacheInvalidator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _productCacheInvalidator = productCacheInvalidator;
        }

        public async Task<Guid> Handle(CreateProductPriceCommand request, CancellationToken cancellationToken)
        {
            if (request.ProductId == Guid.Empty)
                throw new Exception("ProductId is required.");

            if (string.IsNullOrWhiteSpace(request.Currency))
                throw new Exception("Currency is required.");

            if (request.OriginalPrice < 0 || request.SalePrice < 0 || request.CostPrice < 0)
                throw new Exception("Price must be greater than or equal to 0.");

            if (request.EndDate.HasValue && request.StartDate.HasValue && request.EndDate < request.StartDate)
                throw new Exception("EndDate must be greater than StartDate.");

            if (await _repository.ExistsActivePriceAsync(
                    request.ProductId,
                    request.ProductVariantId,
                    null,
                    null,
                    cancellationToken))
            {
                throw new Exception("Active price already exists for this product or variant.");
            }

            var price = new ProductPrice(
                request.ProductId,
                request.ProductVariantId,
                request.Currency,
                request.OriginalPrice,
                request.SalePrice,
                request.CostPrice,
                request.StartDate,
                request.EndDate);

            await _repository.AddAsync(price, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _productCacheInvalidator.ClearProductDetailAsync(request.ProductId, cancellationToken);
            return price.Id;
        }
    }
}
