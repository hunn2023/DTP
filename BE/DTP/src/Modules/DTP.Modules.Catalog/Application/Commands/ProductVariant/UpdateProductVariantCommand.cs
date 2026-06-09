using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Infrastructure.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductVariant
{

    public class UpdateProductVariantCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public string? Sku { get; set; }

        public string Name { get; set; } = default!;

        public string? ShortName { get; set; }

        public string? Description { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }


    public class UpdateProductVariantCommandHandler
    : IRequestHandler<UpdateProductVariantCommand, Result>
    {
        private readonly IProductVariantService _service;

        public UpdateProductVariantCommandHandler(IProductVariantService service)
        {
            _service = service;
        }

        public async Task<Result> Handle(
            UpdateProductVariantCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(
                request.Id,
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
