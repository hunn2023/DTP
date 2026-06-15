using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class ProvisionProviderEsimProductResult
    {
        public Guid ProductId { get; set; }
        public Guid ProductVariantId { get; set; }
        public Guid? ProductPriceId { get; set; }
        public Guid EsimPackageId { get; set; }
    }
}
