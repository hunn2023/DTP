using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductVariant
{
    public class DeleteProductVariantCommand : IRequest<bool>
    {
        public Guid Id { get; }

        public DeleteProductVariantCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteProductVariantCommandHandler
    : IRequestHandler<DeleteProductVariantCommand, bool>
    {
        private readonly IProductVariantService _service;

        public DeleteProductVariantCommandHandler(IProductVariantService service)
        {
            _service = service;
        }

        public async Task<bool> Handle(
            DeleteProductVariantCommand request,
            CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(request.Id, cancellationToken);
            return true;
        }
    }
}
