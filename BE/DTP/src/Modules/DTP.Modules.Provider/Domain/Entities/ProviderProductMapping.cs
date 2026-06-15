using Azure.Core;
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

        public Guid ProviderId { get; private set; }

        public string ProviderSku { get; private set; } = default!;
        public string? ProviderProductId { get; private set; }

        public Guid ProductId { get; private set; }
        public Guid ProductVariantId { get; private set; }
        public Guid? ProductPriceId { get; private set; }
        public Guid EsimPackageId { get; private set; }

        public string MappingStatus { get; private set; } = default!;
        // Inactive, Active, Failed



        public ProviderProductMapping(
            Guid providerId,
            string providerSku,
            string? providerProductId,
            Guid productId,
            Guid productVariantId,
            Guid? productPriceId,
            Guid esimPackageId)
        {
            Id = Guid.NewGuid();
            ProviderId = providerId;
            ProviderSku = providerSku.Trim();
            ProviderProductId = providerProductId;
            ProductId = productId;
            ProductVariantId = productVariantId;
            ProductPriceId = productPriceId;
            EsimPackageId = esimPackageId;
            MappingStatus = "Inactive";
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateCatalogMapping(
            Guid productId,
            Guid productVariantId,
            Guid? productPriceId,
            Guid esimPackageId)
        {
            ProductId = productId;
            ProductVariantId = productVariantId;
            ProductPriceId = productPriceId;
            EsimPackageId = esimPackageId;
            UpdatedAt = DateTime.UtcNow;
        }


        public void Update(string ProviderProductCode,string ProviderProductName,decimal? ProviderCostPrice,string? CurrencyCode,bool IsActive)
        {

            //mapping.Update(
            //    request.ProviderProductCode,
            //    request.ProviderProductName,
            //    request.ProviderCostPrice,
            //    request.CurrencyCode,
            //    request.IsActive);
        }

        public void Activate()
        {
            MappingStatus = "Active";
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            MappingStatus = "Inactive";
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed()
        {
            MappingStatus = "Failed";
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
