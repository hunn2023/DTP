using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductAttributes
{
    public class UpdateProductAttributeCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Value { get; set; } = default!;

        public int SortOrder { get; set; }
    }

    public class UpdateProductAttributeCommandHandler
    : IRequestHandler<UpdateProductAttributeCommand, Result>
    {
        private readonly IProductAttributeService _service;

        public UpdateProductAttributeCommandHandler(IProductAttributeService service)
        {
            _service = service;
        }

        public async Task<Result> Handle(
            UpdateProductAttributeCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(
                request.Id,
                request.Name,
                request.Value,
                request.SortOrder,
                cancellationToken);

        }
    }
}
