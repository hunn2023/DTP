using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public class CreatePaymentQrRequestDto
    {
        public Guid OrderId { get; set; }
    }
}
