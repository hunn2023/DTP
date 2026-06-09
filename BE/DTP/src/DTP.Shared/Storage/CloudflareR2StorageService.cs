using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;


namespace DTP.Shared.Storage
{
    public class CloudflareR2StorageService : IFileStorageService
    {
        private readonly CloudflareR2Settings _settings;
        private readonly IAmazonS3 _s3Client;

        private static readonly string[] AllowedImageTypes =
        {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

        public CloudflareR2StorageService(IOptions<CloudflareR2Settings> options)
        {
            _settings = options.Value;

            var config = new AmazonS3Config
            {
                ServiceURL = "https://ea7bbbc99d611cd10909f552f75520b8.r2.cloudflarestorage.com",
                ForcePathStyle = true,
                AuthenticationRegion = "auto"
            };

            _s3Client = new AmazonS3Client(
                _settings.AccessKey,
                _settings.SecretKey,
                config);
        }

        public async Task<FileUploadResultDto> UploadImageAsync(
             IFormFile file,
             string folder,
             CancellationToken cancellationToken = default)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File không hợp lệ.");

                if (!AllowedImageTypes.Contains(file.ContentType))
                    throw new ArgumentException("Chỉ cho phép upload ảnh jpeg, png, webp hoặc gif.");

                const long maxFileSize = 5 * 1024 * 1024;

                if (file.Length > maxFileSize)
                    throw new ArgumentException("Dung lượng ảnh tối đa là 5MB.");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(extension))
                    throw new ArgumentException("File không có phần mở rộng hợp lệ.");

                var fileName = $"{Guid.NewGuid():N}{extension}";

                folder = string.IsNullOrWhiteSpace(folder)
                    ? "uploads"
                    : folder.Trim('/');

                var key = $"{folder}/{DateTime.UtcNow:yyyy/MM/dd}/{fileName}";

                await using var stream = file.OpenReadStream();

                var request = new PutObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = key,
                    InputStream = stream,
                    ContentType = file.ContentType,

                    
                    // Fix lỗi:
                    // STREAMING-AWS4-HMAC-SHA256-PAYLOAD-TRAILER not implemented
                    DisablePayloadSigning = true,
                    DisableDefaultChecksumValidation = true,

                    AutoCloseStream = true
                };


                await _s3Client.PutObjectAsync(request, cancellationToken);

                var url = $"{_settings.PublicUrl.TrimEnd('/')}/{key}";

                return new FileUploadResultDto
                {
                    FileName = file.FileName,
                    Key = key,
                    Url = url,
                    ContentType = file.ContentType,
                    Size = file.Length
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"Upload ảnh lên Cloudflare R2 thất bại: {ex.Message}", ex);
            }
            catch (AmazonServiceException ex)
            {
                throw new Exception($"Lỗi dịch vụ lưu trữ khi upload ảnh: {ex.Message}", ex);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Có lỗi xảy ra khi upload ảnh: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            var request = new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request, cancellationToken);
        }
    }
}
