using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductVariant
{
    public class DeleteProductVariantCommand : IRequest<Result>
    {
        public Guid Id { get; }

        public DeleteProductVariantCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteProductVariantCommandHandler
    : IRequestHandler<DeleteProductVariantCommand, Result>
    {
        private readonly IProductVariantService _service;

        public DeleteProductVariantCommandHandler(IProductVariantService service)
        {
            _service = service;
        }

        public async Task<Result> Handle(
            DeleteProductVariantCommand request,
            CancellationToken cancellationToken)
        {

            return await _service.DeleteAsync(request.Id, cancellationToken);

        }
    }
}
