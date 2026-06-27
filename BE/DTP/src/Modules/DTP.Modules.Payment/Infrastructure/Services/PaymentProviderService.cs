using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Payment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public class PaymentProviderService : IPaymentProviderService
    {
        private readonly IPaymentProviderRepository _paymentProviderRepository;
        private readonly IPaymentUnitOfWork _unitOfWork;

        public PaymentProviderService(
            IPaymentProviderRepository paymentProviderRepository,
            IPaymentUnitOfWork unitOfWork)
        {
            _paymentProviderRepository = paymentProviderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<PaymentProviderPublicDto>> GetPublicActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var providers = await _paymentProviderRepository.GetPublicActiveAsync(cancellationToken);

            return providers
                .Select(x => new PaymentProviderPublicDto
                {
                    Code = x.Code,
                    Name = x.Name,
                    PaymentMethod = x.PaymentMethod,
                    Currency = x.Currency,
                    IsDefault = x.IsDefault,
                    MinAmount = x.MinAmount,
                    MaxAmount = x.MaxAmount,
                    LogoUrl = x.LogoUrl,
                    Description = x.Description
                })
                .ToList();
        }

        public async Task<IReadOnlyList<PaymentProviderAdminDto>> GetAdminListAsync(
            CancellationToken cancellationToken = default)
        {
            var providers = await _paymentProviderRepository.GetAdminListAsync(cancellationToken);

            return providers
                .Select(x => new PaymentProviderAdminDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    PaymentMethod = x.PaymentMethod,
                    IsActive = x.IsActive,
                    IsDefault = x.IsDefault,
                    SortOrder = x.SortOrder,
                    MinAmount = x.MinAmount,
                    MaxAmount = x.MaxAmount,
                    Currency = x.Currency,
                    LogoUrl = x.LogoUrl,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToList();
        }

        public async Task SetActiveAsync(
            Guid id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var provider = await _paymentProviderRepository.GetByIdAsync(id, cancellationToken);

            if (provider is null)
                throw new KeyNotFoundException("Không tìm thấy phương thức thanh toán.");

            if (isActive)
            {
                provider.Activate();

                var hasDefault = await _paymentProviderRepository
                    .HasActiveDefaultAsync(cancellationToken);

                if (!hasDefault)
                {
                    provider.SetDefault();
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            if (provider.IsDefault)
            {
                var alternativeProvider = await _paymentProviderRepository
                    .GetAlternativeActiveProviderAsync(provider.Id, cancellationToken);

                if (alternativeProvider is null)
                {
                    throw new InvalidOperationException(
                        "Không thể tắt phương thức thanh toán mặc định cuối cùng. Hãy bật phương thức khác trước.");
                }

                provider.RemoveDefault();
                alternativeProvider.SetDefault();
            }

            provider.Deactivate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task SetDefaultAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var provider = await _paymentProviderRepository.GetByIdAsync(id, cancellationToken);

            if (provider is null)
                throw new KeyNotFoundException("Không tìm thấy phương thức thanh toán.");

            if (!provider.IsActive)
                throw new InvalidOperationException("Không thể đặt mặc định cho phương thức thanh toán đang tắt.");

            var allProviders = await _paymentProviderRepository.GetAllTrackingAsync(cancellationToken);

            foreach (var item in allProviders)
            {
                item.RemoveDefault();
            }

            provider.SetDefault();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateLimitsAsync(
            Guid id,
            decimal? minAmount,
            decimal? maxAmount,
            CancellationToken cancellationToken = default)
        {
            var provider = await _paymentProviderRepository.GetByIdAsync(id, cancellationToken);

            if (provider is null)
                throw new KeyNotFoundException("Không tìm thấy phương thức thanh toán.");

            provider.UpdateLimits(minAmount, maxAmount);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateSortOrderAsync(
            Guid id,
            int sortOrder,
            CancellationToken cancellationToken = default)
        {
            var provider = await _paymentProviderRepository.GetByIdAsync(id, cancellationToken);

            if (provider is null)
                throw new KeyNotFoundException("Không tìm thấy phương thức thanh toán.");

            provider.UpdateSortOrder(sortOrder);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<PaymentProvider> GetActiveProviderForCheckoutAsync(
            string? providerCode,
            decimal amount,
            string currency,
            CancellationToken cancellationToken = default)
        {
            PaymentProvider? provider;

            if (string.IsNullOrWhiteSpace(providerCode))
            {
                provider = await _paymentProviderRepository.GetDefaultActiveAsync(cancellationToken);
            }
            else
            {
                provider = await _paymentProviderRepository.GetActiveByCodeAsync(
                    providerCode.Trim().ToUpperInvariant(),
                    cancellationToken);
            }

            if (provider is null)
                throw new InvalidOperationException("Phương thức thanh toán hiện không khả dụng.");

            if (!provider.Currency.Equals(currency, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Phương thức thanh toán không hỗ trợ loại tiền tệ này.");

            if (provider.MinAmount.HasValue && amount < provider.MinAmount.Value)
            {
                throw new InvalidOperationException(
                    $"Số tiền thanh toán nhỏ hơn mức tối thiểu {provider.MinAmount.Value:N0} {provider.Currency}.");
            }

            if (provider.MaxAmount.HasValue && amount > provider.MaxAmount.Value)
            {
                throw new InvalidOperationException(
                    $"Số tiền thanh toán vượt quá mức tối đa {provider.MaxAmount.Value:N0} {provider.Currency}.");
            }

            return provider;
        }
    }
}
