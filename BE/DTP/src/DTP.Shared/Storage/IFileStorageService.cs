using Microsoft.AspNetCore.Http;

namespace DTP.Shared.Storage
{
    public interface IFileStorageService
    {
        Task<FileUploadResultDto> UploadImageAsync(
            IFormFile file,
            string folder,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string key,
            CancellationToken cancellationToken = default);
    }
}
