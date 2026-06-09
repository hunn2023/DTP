using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.ProductImages
{
    public class UpdateProductImageCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public string ImageUrl { get; set; } = default!;

        public string? AltText { get; set; }

        public int SortOrder { get; set; }

        public bool IsThumbnail { get; set; }
    }

    public class UpdateProductImageCommandHandler
    : IRequestHandler<UpdateProductImageCommand, Result>
    {
        private readonly IProductImageService _service;

        public UpdateProductImageCommandHandler(IProductImageService service)
        {
            _service = service;
        }

        public async Task<Result> Handle(
            UpdateProductImageCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(
                 request.Id,
                 request.ImageUrl,
                 request.AltText,
                 request.SortOrder,
                 request.IsThumbnail,
                 cancellationToken);

        }
    }
}
