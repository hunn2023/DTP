using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.Products
{
    public class UpdateProductCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public string? Code { get; set; }

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public Guid CategoryId { get; set; }

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public string? ThumbnailUrl { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }


    public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductService _productService;

        public UpdateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<bool> Handle(
            UpdateProductCommand request,
            CancellationToken cancellationToken)
        {
            return await _productService.UpdateAsync(
               request,
               cancellationToken);
        }
    }
}
