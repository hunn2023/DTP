

namespace DTP.Shared.Storage
{
    public class FileUploadResultDto
    {
        public string FileName { get; set; } = default!;

        public string Key { get; set; } = default!;

        public string Url { get; set; } = default!;

        public string ContentType { get; set; } = default!;

        public long Size { get; set; }
    }
}
