using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class ProductAttributeDto
    {
        public Guid Id { get; set; }

        public Guid? ProductId { get; set; }
        public string Key { get; set; } = default!;
        public string Value { get; set; } = default!;
        public int SortOrder { get; set; }

        public bool? IsVisible { get; set; }
    }
}
