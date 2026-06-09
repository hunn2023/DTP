using DTP.Modules.Customer.Domain.Enums;
using DTP.Shared.Domain;

namespace DTP.Modules.Customer.Domain.Entities
{
    public class Customer : EntityBase
    {
        private readonly List<CustomerAddress> _addresses = new();

        private Customer()
        {
        }

        public Customer(
            Guid userId,
            string email,
            string? fullName,
            string? phoneNumber)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Email = email.Trim().ToLower();
            FullName = fullName?.Trim();
            PhoneNumber = phoneNumber?.Trim();
            Status = CustomerStatus.Active;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid UserId { get; private set; }

        public string Email { get; private set; } = null!;

        public string? FullName { get; private set; }

        public string? PhoneNumber { get; private set; }

        public DateTime? DateOfBirth { get; private set; }

        public string? AvatarUrl { get; private set; }

        public string? Note { get; private set; }

        public CustomerStatus Status { get; private set; }

        public DateTime? LastLoginAt { get; private set; }

        public IReadOnlyCollection<CustomerAddress> Addresses => _addresses;

        public void UpdateProfile(
            string? fullName,
            string? phoneNumber,
            DateTime? dateOfBirth,
            string? avatarUrl)
        {
            FullName = fullName?.Trim();
            PhoneNumber = phoneNumber?.Trim();
            DateOfBirth = dateOfBirth;
            AvatarUrl = avatarUrl?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateAdminInfo(
            string email,
            string? fullName,
            string? phoneNumber,
            DateTime? dateOfBirth,
            string? avatarUrl,
            string? note)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            Email = email.Trim().ToLower();
            FullName = fullName?.Trim();
            PhoneNumber = phoneNumber?.Trim();
            DateOfBirth = dateOfBirth;
            AvatarUrl = avatarUrl?.Trim();
            Note = note?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeStatus(CustomerStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
