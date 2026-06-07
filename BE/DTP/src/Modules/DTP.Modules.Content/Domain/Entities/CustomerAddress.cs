using DTP.Shared.Domain;

namespace DTP.Modules.Customer.Domain.Entities
{
    public class CustomerAddress : EntityBase
    {
        private CustomerAddress()
        {
        }

        public CustomerAddress(
            Guid customerId,
            string receiverName,
            string phoneNumber,
            string addressLine,
            string? ward,
            string? district,
            string? city,
            string? countryCode,
            bool isDefault)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            ReceiverName = receiverName.Trim();
            PhoneNumber = phoneNumber.Trim();
            AddressLine = addressLine.Trim();
            Ward = ward?.Trim();
            District = district?.Trim();
            City = city?.Trim();
            CountryCode = string.IsNullOrWhiteSpace(countryCode)
                ? "VN"
                : countryCode.Trim().ToUpper();

            IsDefault = isDefault;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid CustomerId { get; private set; }

        public string ReceiverName { get; private set; } = null!;

        public string PhoneNumber { get; private set; } = null!;

        public string AddressLine { get; private set; } = null!;

        public string? Ward { get; private set; }

        public string? District { get; private set; }

        public string? City { get; private set; }

        public string CountryCode { get; private set; } = "VN";

        public bool IsDefault { get; private set; }

        public Customer Customer { get; private set; } = null!;

        public void Update(
            string receiverName,
            string phoneNumber,
            string addressLine,
            string? ward,
            string? district,
            string? city,
            string? countryCode)
        {
            ReceiverName = receiverName.Trim();
            PhoneNumber = phoneNumber.Trim();
            AddressLine = addressLine.Trim();
            Ward = ward?.Trim();
            District = district?.Trim();
            City = city?.Trim();
            CountryCode = string.IsNullOrWhiteSpace(countryCode)
                ? "VN"
                : countryCode.Trim().ToUpper();

            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDefault()
        {
            IsDefault = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UnsetDefault()
        {
            IsDefault = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
