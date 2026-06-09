using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductVariantFeatures
{
    public class UpdateProductVariantFeatureCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public string Text { get; set; } = default!;

        public string? Icon { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }


    public class UpdateProductVariantFeatureCommandHandler
      : IRequestHandler<UpdateProductVariantFeatureCommand, Result<bool>>
    {
        private readonly IProductVariantFeatureService _service;

        public UpdateProductVariantFeatureCommandHandler(
            IProductVariantFeatureService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(
            UpdateProductVariantFeatureCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(
                request.Id,
                request.Text,
                request.Icon,
                request.SortOrder,
                request.IsActive,
                cancellationToken);
        }
    }
}
