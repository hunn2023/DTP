using DTP.Modules.Provider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProviderProductMappingDto
    {
        public Guid Id { get; set; }

        public Guid ProviderId { get; set; }

        public string ProviderName { get; set; } = default!;

        public ProviderProductType ProductType { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductVariantId { get; set; }

        public string ProviderProductCode { get; set; } = default!;

        public string? ProviderProductName { get; set; }

        public decimal? ProviderCostPrice { get; set; }

        public string? CurrencyCode { get; set; }

        public bool IsActive { get; set; }
    }
}
