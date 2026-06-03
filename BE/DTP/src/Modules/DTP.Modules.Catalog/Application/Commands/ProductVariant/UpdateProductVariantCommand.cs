using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductVariant
{

    public class UpdateProductVariantCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string? Sku { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int? DurationDays { get; set; }
        public decimal? DataAmount { get; set; }
        public string? DataUnit { get; set; }
        public bool IsUnlimited { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }


    public class UpdateProductVariantCommandHandler
    : IRequestHandler<UpdateProductVariantCommand, bool>
    {
        private readonly IProductVariantService _service;

        public UpdateProductVariantCommandHandler(IProductVariantService service)
        {
            _service = service;
        }

        public async Task<bool> Handle(
            UpdateProductVariantCommand request,
            CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(
                request.Id,
                request.Sku,
                request.Name,
                request.Price,
                request.OriginalPrice,
                request.DurationDays,
                request.DataAmount,
                request.DataUnit,
                request.IsUnlimited,
                request.SortOrder,
                request.IsActive,
                cancellationToken);

            return true;
        }
    }
}
