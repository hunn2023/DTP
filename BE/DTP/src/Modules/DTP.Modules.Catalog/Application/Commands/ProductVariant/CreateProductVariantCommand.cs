using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductVariant
{
    public class CreateProductVariantCommand : IRequest<Result<Guid>>
    {
        public Guid ProductId { get; set; }
        public string? Sku { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int? DurationDays { get; set; }
        public decimal? DataAmount { get; set; }
        public string? DataUnit { get; set; }
        public bool IsUnlimited { get; set; }
        public int SortOrder { get; set; }
    }

    public class CreateProductVariantCommandHandler
    : IRequestHandler<CreateProductVariantCommand, Result<Guid>>
    {
        private readonly IProductVariantService _service;

        public CreateProductVariantCommandHandler(IProductVariantService service)
        {
            _service = service;
        }

        public async Task<Result<Guid>> Handle(
            CreateProductVariantCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(
                request.ProductId,
                request.Sku,
                request.Name,
                request.Price,
                request.OriginalPrice,
                request.DurationDays,
                request.DataAmount,
                request.DataUnit,
                request.IsUnlimited,
                request.SortOrder,
                cancellationToken);
        }
    }
}
