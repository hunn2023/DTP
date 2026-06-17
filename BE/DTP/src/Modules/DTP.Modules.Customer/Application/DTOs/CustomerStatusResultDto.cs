namespace DTP.Modules.Customer.Application.DTOs
{
    public class CustomerStatusResultDto
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string StatusText => IsActive ? "Hoạt động" : "Đã khóa";
    }
}
