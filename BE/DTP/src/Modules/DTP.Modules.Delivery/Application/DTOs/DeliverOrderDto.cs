using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.DTOs
{
    public class DeliverOrderDto
    {
        public Guid OrderId { get; set; }

        public string? Note { get; set; }
    }
}
