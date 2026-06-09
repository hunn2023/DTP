using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class ProviderDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Slug { get; set; }

        public string? LogoUrl { get; set; } = default!;

        public string? WebsiteUrl { get; set; }
        public string? ApiBaseUrl { get; set; }

        public string? ApiKey { get; set; }

        public string? ApiSecret { get; set; }

        public string? WebhookUrl { get; set; }

        public string? SupportEmail { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
