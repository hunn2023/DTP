

using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using Microsoft.AspNetCore.Http;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{

    public interface IProductImageService
    {
        Task<Result<Guid>> CreateAsync(
            Guid productId,
            string imageUrl,
            string? altText,
            int sortOrder,
            bool isThumbnail,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            Guid id,
            string imageUrl,
            string? altText,
            int sortOrder,
            bool isThumbnail,
            CancellationToken cancellationToken = default);

   


        Task<Result> ReplaceImageAsync(
            Guid productId,
            Guid imageId,
            IFormFile file,
            CancellationToken cancellationToken = default);


        Task<Result> SetThumbnailAsync(
            Guid productId,
            Guid imageId,
            CancellationToken cancellationToken = default);


        Task<Result<ProductImageDto>> UploadAsync(
           Guid productId,
           IFormFile file,
           string? altText,
           bool isThumbnail,
           CancellationToken cancellationToken = default);


        Task<Result> DeleteAsync(
           Guid productId,
           Guid imageId,
           CancellationToken cancellationToken = default);
    }
}
