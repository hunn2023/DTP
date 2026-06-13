using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.DTOs
{
    public class CustomerAddressDto
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public string ReceiverName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string AddressLine { get; set; } = null!;

        public string? Ward { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }

        public string CountryCode { get; set; } = "VN";

        public bool IsDefault { get; set; }
    }
}
