

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class PhoneCardDto
    {
        public Guid Id { get; set; }

        public Guid ProductVariantId { get; set; }
        public string? ProductVariantName { get; set; }

        public Guid ProviderId { get; set; }
        public string? ProviderName { get; set; }

        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;

        public decimal FaceValue { get; set; }
        public decimal Price { get; set; }

        public string Currency { get; set; } = default!;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
