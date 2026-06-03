using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.Products
{
    public class GetProductDetailQuery : IRequest<ProductDto?>
    {
        public Guid Id { get; set; }

        public GetProductDetailQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetProductDetailQueryHandler
    : IRequestHandler<GetProductDetailQuery, ProductDto?>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductService _productService;

        public GetProductDetailQueryHandler(IProductRepository productRepository, IProductService productService)
        {
            _productRepository = productRepository;
            _productService = productService;
        }

        public async Task<ProductDto?> Handle(
            GetProductDetailQuery request,
            CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                Slug = product.Slug,
                CategoryId = product.CategoryId,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                ThumbnailUrl = product.ThumbnailUrl,
                SortOrder = product.SortOrder,
                IsActive = product.IsActive,

                Variants = product.Variants
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new ProductVariantDto
                    {
                        Id = x.Id,
                        Sku = x.Sku,
                        Name = x.Name,
                        Price = x.Price,
                        OriginalPrice = x.OriginalPrice,
                        DurationDays = x.DurationDays,
                        DataAmount = x.DataAmount,
                        DataUnit = x.DataUnit,
                        IsUnlimited = x.IsUnlimited,
                        SortOrder = x.SortOrder,
                        IsActive = x.IsActive
                    }).ToList(),

                Images = product.Images
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new ProductImageDto
                    {
                        Id = x.Id,
                        ImageUrl = x.ImageUrl,
                        AltText = x.AltText,
                        SortOrder = x.SortOrder,
                        IsThumbnail = x.IsThumbnail
                    }).ToList(),

                Attributes = product.Attributes
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new ProductAttributeDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Value = x.Value,
                        SortOrder = x.SortOrder
                    }).ToList()
            };
        }
    }
}
