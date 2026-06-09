using DTP.Modules.Provider.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class ProviderProductMapping : EntityBase
    {
        private ProviderProductMapping()
        {
        }

        public ProviderProductMapping(
            Guid providerId,
            ProviderProductType productType,
            Guid productId,
            Guid productVariantId,
            string providerProductCode,
            string? providerProductName,
            bool isActive)
        {
            Id = Guid.NewGuid();
            ProviderId = providerId;
            ProductType = productType;
            ProductId = productId;
            ProductVariantId = productVariantId;
            ProviderProductCode = providerProductCode.Trim();
            ProviderProductName = providerProductName?.Trim();
            IsActive = isActive;
        }

        public Guid ProviderId { get; private set; }

        public ExternalProvider Provider { get; private set; } = default!;

        public ProviderProductType ProductType { get; private set; }

        public Guid ProductId { get; private set; }

        public Guid ProductVariantId { get; private set; }

        public string ProviderProductCode { get; private set; } = default!;

        public string? ProviderProductName { get; private set; }

        public decimal? ProviderCostPrice { get; private set; }

        public string? CurrencyCode { get; private set; }

        public bool IsActive { get; private set; }

        public void Update(
            string providerProductCode,
            string? providerProductName,
            decimal? providerCostPrice,
            string? currencyCode,
            bool isActive)
        {
            ProviderProductCode = providerProductCode.Trim();
            ProviderProductName = providerProductName?.Trim();
            ProviderCostPrice = providerCostPrice;
            CurrencyCode = currencyCode?.Trim();
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
