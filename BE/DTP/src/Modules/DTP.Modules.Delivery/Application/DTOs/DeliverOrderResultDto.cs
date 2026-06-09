using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.DTOs
{
    public class DeliverOrderResultDto
    {
        public Guid OrderId { get; set; }

        public bool Success { get; set; }

        public string? Message { get; set; }

        public List<Guid> EsimProfileIds { get; set; } = new();
    }
}
