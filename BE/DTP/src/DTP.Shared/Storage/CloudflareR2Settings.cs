using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Storage
{
    public class CloudflareR2Settings
    {
        public string AccountId { get; set; } = default!;

        public string AccessKey { get; set; } = default!;

        public string SecretKey { get; set; } = default!;

        public string BucketName { get; set; } = default!;

        public string PublicUrl { get; set; } = default!;

        public string ServiceUrl { get; set; } = default!;
    }
}
