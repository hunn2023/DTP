using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs.Peacoms
{
    public class PeacomCreateOrderRequest
    {
        public List<PeacomCreateOrderProductDto> Products { get; set; } = new();

        public string RequestId { get; set; }
    }

    public class PeacomCreateOrderProductDto
    {

        public string Sku { get; set; } = default!;

        public int Quantity { get; set; }
    }

    public class PeacomCreateOrderResponse
    {
        public bool Success { get; set; }

        public int? OrderId { get; set; }

        public string OrderPublicId { get; set; } = default!;

        public decimal Amount { get; set; }

        public int NumOfProduct { get; set; }

        public int Status { get; set; }

        public string RawJson { get; set; } = default!;
    }
}
