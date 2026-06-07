using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Domain.Entities
{
    public class PendingRegistration : EntityBase
    {

        public string Email { get; set; } = default!;
        public string? Phone { get; set; }

        public string FullName { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;

        public string OtpCodeHash { get; set; } = default!;
        public DateTime OtpExpiredAt { get; set; }

        public int VerifyFailedCount { get; set; }

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

    }
}
