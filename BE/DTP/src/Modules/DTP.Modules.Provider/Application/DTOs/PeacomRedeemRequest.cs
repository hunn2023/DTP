using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class PeacomRedeemRequest
    {
        public string Serial { get; set; } = default!;
        public PeacomInfoCustomerDto InfoCustomer { get; set; } = new();
    }

    public class PeacomInfoCustomerDto
    {
        public string CustomerPhone { get; set; } = default!;
        public string CustomerEmail { get; set; } = default!;
        public string CustomerName { get; set; } = default!;

        public string? CustomerDob { get; set; }
        public string? CustomerCccd { get; set; }
        public string? CustomerGender { get; set; }
        public string? CustomerAddress { get; set; }

        public string? DepartureDate { get; set; }
        public string? ReturnDate { get; set; }

        public List<PeacomRedeemCountryDto>? Country { get; set; }
        public List<PeacomAttachedPersonDto>? AttachedPersons { get; set; }
    }

    public class PeacomRedeemCountryDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = default!;
        public string Alpha2Code { get; set; } = default!;
        public string? Alpha3Code { get; set; }
    }

    public class PeacomAttachedPersonDto
    {
        public string Name { get; set; } = default!;
        public string? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Cccd { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }

    public class PeacomRedeemResponse
    {
        public bool Success { get; set; }
        public string Serial { get; set; } = default!;
        public DateTime? RequestTime { get; set; }
        public int Status { get; set; }

        public string RawJson { get; set; } = default!;
    }
}
