using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductPrices
{
    public class UpdateProductPriceCommand : IRequest<Result>
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


    public class UpdateProductPriceCommandHandler : IRequestHandler<UpdateProductPriceCommand, Result>
    {
        private readonly IProductPriceService _productPriceService;

        public UpdateProductPriceCommandHandler(
            IProductPriceService productPriceService
            )
        {
            _productPriceService = productPriceService;
        }

        public async Task<Result> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
        {
           return await _productPriceService.UpdateAsync(
                request.Id,
                request.Currency,
                request.OriginalPrice,
                request.SalePrice,
                request.CostPrice,
                request.StartDate,
                request.EndDate,
                request.IsActive,
                cancellationToken);
        }
    }
}
