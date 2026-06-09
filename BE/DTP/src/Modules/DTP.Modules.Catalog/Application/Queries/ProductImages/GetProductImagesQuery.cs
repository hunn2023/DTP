using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.ProductImages
{
    public class GetProductImagesQuery : IRequest<Result<List<ProductImageDto>>>
    {
        public Guid ProductId { get; }

        public GetProductImagesQuery(Guid productId)
        {
            ProductId = productId;
        }
    }


    public class GetProductImagesQueryHandler
        : IRequestHandler<GetProductImagesQuery, Result<List<ProductImageDto>>>
    {
        private readonly IProductImageRepository _productImageRepository;

        public GetProductImagesQueryHandler(
            IProductImageRepository productImageRepository)
        {
            _productImageRepository = productImageRepository;
        }

        public async Task<Result<List<ProductImageDto>>> Handle(
            GetProductImagesQuery request,
            CancellationToken cancellationToken)
        {
            var images = await _productImageRepository.GetByProductIdAsync(
                request.ProductId,
                cancellationToken);

            var result = images.Select(x => new ProductImageDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ImageUrl = x.ImageUrl,
                AltText = x.AltText,
                SortOrder = x.SortOrder,
                IsThumbnail = x.IsThumbnail
            }).ToList();

            return Result<List<ProductImageDto>>.Success(result);
        }
    }
}
