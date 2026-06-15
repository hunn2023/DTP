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

        public Guid ProductId { get; set; }
        public Guid ProductVariantId { get; set; }
        public Guid? ProductPriceId { get; set; }
        public Guid EsimPackageId { get; set; }

        public string MappingStatus { get; set; } = default!;
    }
}
