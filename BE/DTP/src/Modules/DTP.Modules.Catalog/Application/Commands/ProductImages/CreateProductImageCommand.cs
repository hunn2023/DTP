using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.ProductImages
{
    public class CreateProductImageCommand : IRequest<Result<Guid>>
    {
        public Guid ProductId { get; set; }

        public string ImageUrl { get; set; } = default!;

        public string? AltText { get; set; }

        public int SortOrder { get; set; }

        public bool IsThumbnail { get; set; }
    }

    public class CreateProductImageCommandHandler
    : IRequestHandler<CreateProductImageCommand, Result<Guid>>
    {
        private readonly IProductImageService _service;

        public CreateProductImageCommandHandler(IProductImageService service)
        {
            _service = service;
        }

        public async Task<Result<Guid>> Handle(
            CreateProductImageCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(
                request.ProductId,
                request.ImageUrl,
                request.AltText,
                request.SortOrder,
                request.IsThumbnail,
                cancellationToken);
        }
    }
}
