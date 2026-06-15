using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class PeacomConfirmOrderRequest
    {
        public bool IsConfirm { get; set; }
    }
    public class PeacomConfirmOrderResponse
    {
        public bool Success { get; set; }
        public int OrderId { get; set; }
        public string OrderPublicId { get; set; } = default!;
        public decimal Amount { get; set; }
        public int NumOfProduct { get; set; }
        public int Status { get; set; }
        public List<PeacomConfirmOrderItemDto> Data { get; set; } = new();

        public string RawJson { get; set; } = default!;
    }

    public class PeacomConfirmOrderItemDto
    {
        public int? ProductId { get; set; }
        public string Sku { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public int Qty { get; set; }
        public List<string> Serials { get; set; } = new();
    }
}
