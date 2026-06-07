using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Application.DTOs
{
    public class CustomerListItemDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public CustomerStatus Status { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public int AddressCount { get; set; }
    }
}
