using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Infrastructure.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductVariant
{
    public class CreateProductVariantCommand : IRequest<Result<Guid>>
    {
        public Guid ProductId { get; set; }

        public string? Sku { get; set; }

        public string Name { get; set; } = default!;

        public string? ShortName { get; set; }

        public string? Description { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
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
               request.ShortName,
               request.Description,
               request.SortOrder,
               request.IsActive,
               cancellationToken);
        }
    }
}
