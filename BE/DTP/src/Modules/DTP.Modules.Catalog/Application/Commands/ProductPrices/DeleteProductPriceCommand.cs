using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Domain.Entities;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductPrices
{
    public class DeleteProductPriceCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteProductPriceCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteProductPriceCommandHandler : IRequestHandler<DeleteProductPriceCommand, bool>
    {
        private readonly IProductPriceRepository _repository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly IProductCacheInvalidator _productCacheInvalidator;
        public DeleteProductPriceCommandHandler(
            IProductPriceRepository repository,
            ICatalogUnitOfWork unitOfWork,
            IProductCacheInvalidator productCacheInvalidator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _productCacheInvalidator = productCacheInvalidator;
        }

        public async Task<bool> Handle(DeleteProductPriceCommand request, CancellationToken cancellationToken)
        {
            var price = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (price == null)
                throw new Exception("Product price not found.");

            price.Deactivate();

            _repository.Update(price);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _productCacheInvalidator.ClearProductDetailAsync(price.ProductId, cancellationToken);
            return true;
        }
    }
}
