

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class EsimPackageDto
    {
        public Guid Id { get; set; }

        public Guid ProductVariantId { get; set; }

        public string ProductVariantName { get; set; } = default!;

        public string ProductName { get; set; } = default!;

        public Guid CountryId { get; set; }

        public string CountryName { get; set; } = default!;

        public Guid CarrierId { get; set; }

        public string CarrierName { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public int DataAmount { get; set; }

        public string DataUnit { get; set; } = default!;

        public int ValidityDays { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; } = default!;

        public bool IsUnlimited { get; set; }

        public bool IsActive { get; set; }

        public int SortOrder { get; set; }
    }
}
