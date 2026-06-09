using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.Products
{
    public class DeleteProductCommand : IRequest<Result>
    {
        public Guid Id { get; }

        public DeleteProductCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, Result>
    {
        private readonly IProductService _productService;

        public DeleteProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result> Handle(
            DeleteProductCommand request,
            CancellationToken cancellationToken)
        {
            return await _productService.DeleteAsync(
                 request.Id,
                 cancellationToken);

        }
    }
}
