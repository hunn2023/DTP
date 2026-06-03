using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.Products
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public string? Code { get; set; }

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public Guid CategoryId { get; set; }

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public string? ThumbnailUrl { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }


    public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductService _productService;
        private readonly IProductCacheInvalidator _productCacheInvalidator; 

        public CreateProductCommandHandler(IProductService productService, IProductCacheInvalidator productCacheInvalidator)
        {
            _productService = productService;
            _productCacheInvalidator = productCacheInvalidator;
        }

        public async Task<Guid> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            return await _productService.CreateAsync(
              request,
              cancellationToken);
        }
    }
}
