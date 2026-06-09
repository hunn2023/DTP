using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductVariantFeatures
{
    public class DeleteProductVariantFeatureCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteProductVariantFeatureCommand(Guid id)
        {
            Id = id;
        }
    }


    public class DeleteProductVariantFeatureCommandHandler
       : IRequestHandler<DeleteProductVariantFeatureCommand, Result<bool>>
    {
        private readonly IProductVariantFeatureService _service;

        public DeleteProductVariantFeatureCommandHandler(
            IProductVariantFeatureService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(
            DeleteProductVariantFeatureCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(
                request.Id,
                cancellationToken);
        }
    }
}
