using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductAttributes
{
    public class DeleteProductAttributeCommand : IRequest<Result>
    {
        public Guid Id { get; }

        public DeleteProductAttributeCommand(Guid id)
        {
            Id = id;
        }
    }


    public class DeleteProductAttributeCommandHandler
    : IRequestHandler<DeleteProductAttributeCommand, Result>
    {
        private readonly IProductAttributeService _service;

        public DeleteProductAttributeCommandHandler(IProductAttributeService service)
        {
            _service = service;
        }

        public async Task<Result> Handle(
            DeleteProductAttributeCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(
               request.Id,
               cancellationToken);
        }
    }
}
