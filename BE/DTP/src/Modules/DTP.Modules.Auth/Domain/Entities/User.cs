using DTP.Shared.Domain;


namespace DTP.Modules.Auth.Domain.Entities
{
    public class User : EntityBase
    {

        public string Email { get; set; } = default!;
        public string? Phone { get; set; }

        public string PasswordHash { get; set; } = default!;

        public string FullName { get; set; } = default!;
        public string? AvatarUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; } = true;
        public bool PhoneConfirmed { get; set; } = true;

        public DateTime? LastLoginAt { get; set; }

        public string? PasswordResetOtpHash { get; set; }
        public DateTime? PasswordResetOtpExpiredAt { get; set; }
        public int PasswordResetVerifyFailedCount { get; set; }


        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public void UpdateProfile(string fullName, string? phone, string? avatarUrl)
        {
            FullName = fullName.Trim();
            Phone = phone;
            AvatarUrl = avatarUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Lock()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unlock()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
