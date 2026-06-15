using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public sealed class CatalogProductProvisioningResult
    {
        public Guid CountryId { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductVariantId { get; set; }

        public Guid ProductPriceId { get; set; }

        public Guid EsimPackageId { get; set; }

        public bool ProductCreated { get; set; }

        public bool VariantCreated { get; set; }

        public bool PriceCreated { get; set; }

        public bool PackageCreated { get; set; }
    }
}
