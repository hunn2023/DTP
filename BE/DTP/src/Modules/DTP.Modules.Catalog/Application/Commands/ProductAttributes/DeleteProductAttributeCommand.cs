using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductAttributes
{
    public class DeleteProductAttributeCommand : IRequest<bool>
    {
        public Guid Id { get; }

        public DeleteProductAttributeCommand(Guid id)
        {
            Id = id;
        }
    }


    public class DeleteProductAttributeCommandHandler
    : IRequestHandler<DeleteProductAttributeCommand, bool>
    {
        private readonly IProductAttributeService _service;

        public DeleteProductAttributeCommandHandler(IProductAttributeService service)
        {
            _service = service;
        }

        public async Task<bool> Handle(
            DeleteProductAttributeCommand request,
            CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(
                request.Id,
                cancellationToken);

            return true;
        }
    }
}
