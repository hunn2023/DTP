using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.ProductImages
{
    public class DeleteProductImageCommand : IRequest<bool>
    {
        public Guid Id { get; }

        public DeleteProductImageCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteProductImageCommandHandler
    : IRequestHandler<DeleteProductImageCommand, bool>
    {
        private readonly IProductImageService _service;

        public DeleteProductImageCommandHandler(IProductImageService service)
        {
            _service = service;
        }

        public async Task<bool> Handle(
            DeleteProductImageCommand request,
            CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(
                request.Id,
                cancellationToken);

            return true;
        }
    }
}
