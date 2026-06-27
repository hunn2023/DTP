using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Domain.Entities
{
    public class PaymentProvider : EntityBase
    {

        public string Code { get; private set; } = default!;

        public string Name { get; private set; } = default!;

        public string PaymentMethod { get; private set; } = default!;

        public bool IsActive { get; private set; }

        public bool IsDefault { get; private set; }

        public int SortOrder { get; private set; }

        public decimal? MinAmount { get; private set; }

        public decimal? MaxAmount { get; private set; }

        public string Currency { get; private set; } = "VND";

        public string? LogoUrl { get; private set; }

        public string? Description { get; private set; }


        private PaymentProvider()
        {
        }

        public PaymentProvider(
            Guid id,
            string code,
            string name,
            string paymentMethod,
            string currency = "VND",
            bool isActive = false,
            bool isDefault = false,
            int sortOrder = 0,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            string? logoUrl = null,
            string? description = null)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException("Mã phương thức thanh toán không được rỗng.");

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Tên phương thức thanh toán không được rỗng.");

            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new InvalidOperationException("Loại thanh toán không được rỗng.");

            ValidateLimits(minAmount, maxAmount);

            Id = id;
            Code = code.Trim().ToUpperInvariant();
            Name = name.Trim();
            PaymentMethod = paymentMethod.Trim();
            Currency = string.IsNullOrWhiteSpace(currency)
                ? "VND"
                : currency.Trim().ToUpperInvariant();

            IsActive = isActive;
            IsDefault = isDefault;
            SortOrder = sortOrder;
            MinAmount = minAmount;
            MaxAmount = maxAmount;
            LogoUrl = logoUrl;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDefault()
        {
            if (!IsActive)
                throw new InvalidOperationException("Không thể đặt mặc định cho phương thức thanh toán đang tắt.");

            IsDefault = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveDefault()
        {
            IsDefault = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLimits(decimal? minAmount, decimal? maxAmount)
        {
            ValidateLimits(minAmount, maxAmount);

            MinAmount = minAmount;
            MaxAmount = maxAmount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateSortOrder(int sortOrder)
        {
            if (sortOrder < 0)
                throw new InvalidOperationException("Thứ tự hiển thị không được nhỏ hơn 0.");

            SortOrder = sortOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        private static void ValidateLimits(decimal? minAmount, decimal? maxAmount)
        {
            if (minAmount.HasValue && minAmount.Value < 0)
                throw new InvalidOperationException("Số tiền tối thiểu không được nhỏ hơn 0.");

            if (maxAmount.HasValue && maxAmount.Value < 0)
                throw new InvalidOperationException("Số tiền tối đa không được nhỏ hơn 0.");

            if (minAmount.HasValue && maxAmount.HasValue && minAmount.Value > maxAmount.Value)
                throw new InvalidOperationException("Số tiền tối thiểu không được lớn hơn số tiền tối đa.");
        }
    }
}
