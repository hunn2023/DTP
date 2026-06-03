using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductPrices
{
    public class UpdateProductPriceCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Currency { get; set; } = "VND";
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }


    public class UpdateProductPriceCommandHandler : IRequestHandler<UpdateProductPriceCommand, bool>
    {
        private readonly IProductPriceRepository _repository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly IProductCacheInvalidator _productCacheInvalidator;
        public UpdateProductPriceCommandHandler(
            IProductPriceRepository repository,
            ICatalogUnitOfWork unitOfWork,
            IProductCacheInvalidator productCacheInvalidator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _productCacheInvalidator = productCacheInvalidator;
        }

        public async Task<bool> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
        {
            var price = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (price == null)
                throw new Exception("Product price not found.");

            if (request.EndDate.HasValue && request.StartDate.HasValue && request.EndDate < request.StartDate)
                throw new Exception("EndDate must be greater than StartDate.");

            if (request.IsActive)
            {
                var exists = await _repository.ExistsActivePriceAsync(
                    price.ProductId,
                    price.ProductVariantId,
                    "VND"   ,
                    price.Id,
                    cancellationToken);

                if (exists)
                    throw new Exception("Another active price already exists for this product or variant.");
            }

            price.Update(
                request.Currency,
                request.OriginalPrice,
                request.SalePrice,
                request.CostPrice,
                request.StartDate,
                request.EndDate,
                request.IsActive);

            _repository.Update(price);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _productCacheInvalidator.ClearProductDetailAsync(price.ProductId, cancellationToken);

            return true;
        }
    }
}
