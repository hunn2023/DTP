using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.ProductImages
{
    public class GetProductImagesQuery : IRequest<List<ProductImageDto>>
    {
        public Guid ProductId { get; }

        public GetProductImagesQuery(Guid productId)
        {
            ProductId = productId;
        }
    }

    public class GetProductImagesQueryHandler
    : IRequestHandler<GetProductImagesQuery, List<ProductImageDto>>
    {
        private readonly IProductImageRepository _repository;

        public GetProductImagesQueryHandler(IProductImageRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductImageDto>> Handle(
            GetProductImagesQuery request,
            CancellationToken cancellationToken)
        {
            var images = await _repository.GetListAsync(
                request.ProductId,
                cancellationToken);

            return images.Select(x => new ProductImageDto
            {
                Id = x.Id,
                ImageUrl = x.ImageUrl,
                AltText = x.AltText,
                SortOrder = x.SortOrder,
                IsThumbnail = x.IsThumbnail
            }).ToList();
        }
    }
}
