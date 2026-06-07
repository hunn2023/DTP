using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductAttributes
{
    public class CreateProductAttributeCommand : IRequest<Result<Guid>>
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; } = default!;

        public string Value { get; set; } = default!;

        public int SortOrder { get; set; }
    }

    public class CreateProductAttributeCommandHandler
    : IRequestHandler<CreateProductAttributeCommand, Result<Guid>>
    {
        private readonly IProductAttributeService _service;

        public CreateProductAttributeCommandHandler(IProductAttributeService service)
        {
            _service = service;
        }

        public async Task<Result<Guid>> Handle(
            CreateProductAttributeCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(
                request.ProductId,
                request.Name,
                request.Value,
                request.SortOrder,
                cancellationToken);
        }
    }

}
