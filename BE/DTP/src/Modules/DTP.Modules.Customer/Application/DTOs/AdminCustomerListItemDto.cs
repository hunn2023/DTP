namespace DTP.Modules.Customer.Application.DTOs
{
    public class AdminCustomerListItemDto
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        public bool IsActive { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public IReadOnlyList<string> Roles { get; set; } = new List<string>();

        public int TotalOrders { get; set; }

        public decimal TotalSpent { get; set; }
    }
}
