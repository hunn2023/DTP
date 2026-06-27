using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public class PaymentProviderPublicDto
    {
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string PaymentMethod { get; set; } = default!;

        public string Currency { get; set; } = "VND";

        public bool IsDefault { get; set; }

        public decimal? MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }

        public string? LogoUrl { get; set; }

        public string? Description { get; set; }
    }

    public class PaymentProviderAdminDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string PaymentMethod { get; set; } = default!;

        public bool IsActive { get; set; }

        public bool IsDefault { get; set; }

        public int SortOrder { get; set; }

        public decimal? MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }

        public string Currency { get; set; } = "VND";

        public string? LogoUrl { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
