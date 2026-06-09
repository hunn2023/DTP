using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Domain.Entities
{
    public class AuthAttemptLog : EntityBase
    {
        public string ActionType { get; set; } = default!;
        public string Identifier { get; set; } = default!;

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        public bool Success { get; set; }
        public string? FailReason { get; set; }

    }
}
