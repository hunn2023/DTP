using DTP.Modules.Provider.Domain.Enums;
using MediatR;


namespace DTP.Modules.Provider.Application.Commands.Mappings
{

    public class CreateProviderProductMappingCommand : IRequest<Guid>
    {
        public Guid ProviderId { get; set; }

        public ProviderProductType ProductType { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductVariantId { get; set; }

        public string ProviderProductCode { get; set; } = default!;

        public string? ProviderProductName { get; set; }

        public decimal? ProviderCostPrice { get; set; }

        public string? CurrencyCode { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
