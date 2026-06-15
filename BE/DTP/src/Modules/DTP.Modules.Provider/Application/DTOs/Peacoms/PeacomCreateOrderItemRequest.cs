using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs.Peacoms
{
    public class PeacomCreateOrderItemRequest
    {
        public int ProductId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public int Qty { get; set; }
    }
}
