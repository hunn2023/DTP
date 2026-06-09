using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.Products
{
    public class UpdateProductCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public string? Code { get; set; }

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public Guid CategoryId { get; set; }

        public Guid? CountryId { get; set; }

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public string? LocationText { get; set; }

        public string? ThumbnailUrl { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsHot { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }


    public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, Result>
    {
        private readonly IProductService _productService;

        public UpdateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result> Handle(
            UpdateProductCommand request,
            CancellationToken cancellationToken)
        {
            return await _productService.UpdateAsync(
               request,
               cancellationToken);
        }
    }
}
