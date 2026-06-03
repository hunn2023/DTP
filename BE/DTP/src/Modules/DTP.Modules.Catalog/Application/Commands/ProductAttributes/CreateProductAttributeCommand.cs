using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductAttributes
{
    public class CreateProductAttributeCommand : IRequest<Guid>
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; } = default!;

        public string Value { get; set; } = default!;

        public int SortOrder { get; set; }
    }

    public class CreateProductAttributeCommandHandler
    : IRequestHandler<CreateProductAttributeCommand, Guid>
    {
        private readonly IProductAttributeService _service;

        public CreateProductAttributeCommandHandler(IProductAttributeService service)
        {
            _service = service;
        }

        public async Task<Guid> Handle(
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
