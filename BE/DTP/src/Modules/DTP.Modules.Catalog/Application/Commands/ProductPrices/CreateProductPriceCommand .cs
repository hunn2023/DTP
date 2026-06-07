using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductPrices
{
    public class CreateProductPriceCommand : IRequest<Result<Guid>>
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

    public class CreateProductPriceCommandHandler : IRequestHandler<CreateProductPriceCommand, Result<Guid>>
    {
        private readonly IProductPriceService _productPriceService;

        public CreateProductPriceCommandHandler( IProductPriceService productPriceService)
        {
            _productPriceService = productPriceService;
        }

        public async Task<Result<Guid>> Handle(CreateProductPriceCommand request, CancellationToken cancellationToken)
        {
            return await _productPriceService.CreateAsync(
                request.ProductId,
                request.ProductVariantId,
                request.Currency,
                request.OriginalPrice,
                request.SalePrice,
                request.CostPrice,
                request.StartDate,
                request.EndDate,
                cancellationToken);
        }
    }
}
