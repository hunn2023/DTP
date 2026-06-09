using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductPrices
{
    public class DeleteProductPriceCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public DeleteProductPriceCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteProductPriceCommandHandler : IRequestHandler<DeleteProductPriceCommand, Result>
    {
        private readonly IProductPriceService _productPriceService;
        public DeleteProductPriceCommandHandler(
            IProductPriceService productPriceService)
        {
            _productPriceService = productPriceService;
        }

        public async Task<Result> Handle(DeleteProductPriceCommand request, CancellationToken cancellationToken)
        {
            return await _productPriceService.DeleteProductPriceAsync(request.Id);
        }
    }
}
