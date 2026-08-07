namespace DTP.Modules.Catalog.Application.DTOs
{
    public class EsimDestinationDetailDto
    {
        public Guid CountryId { get; set; }
        public string CountryCode { get; set; } = default!;
        public string CountryName { get; set; } = default!;
        public string CountrySlug { get; set; } = default!;
        public string? FlagUrl { get; set; }
        public string? Region { get; set; }
        public string? Description { get; set; }
        public int PackageCount { get; set; }
        public decimal PriceFrom { get; set; }
        public string Currency { get; set; } = "VND";
        public List<EsimPackageDto> Packages { get; set; } = new();
    }
}
