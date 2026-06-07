using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Domain.Entities
{
    public class RefreshToken : EntityBase
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public string TokenHash { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }

        public string? CreatedByIp { get; set; }

        public DateTime? RevokedAt { get; set; }
        public string? RevokedByIp { get; set; }

        public string? ReplacedByTokenHash { get; set; }

        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsExpired => DateTime.Now >= ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
