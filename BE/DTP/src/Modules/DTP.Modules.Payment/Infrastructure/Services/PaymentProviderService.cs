using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.Constants;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Payment.Application.Options;
using DTP.Modules.Payment.Domain.Entities;
using Microsoft.Extensions.Options;
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
        private readonly SepayOptions _sepayOptions;

        public PaymentProviderService(
            IPaymentProviderRepository paymentProviderRepository,
            IOptions<SepayOptions> sepayOptions,
            IPaymentUnitOfWork unitOfWork)
        {
            _paymentProviderRepository = paymentProviderRepository;
            _sepayOptions = sepayOptions.Value;
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

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var allProviders = await _paymentProviderRepository.GetAllTrackingAsync(cancellationToken);

                foreach (var item in allProviders.Where(x => x.IsDefault))
                {
                    item.RemoveDefault();
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                provider.SetDefault();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
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



        public async Task<PaymentProviderValidationResult> ValidateForCreateQrAsync(
               string? paymentProviderCode,
               decimal amount,
               string currency,
               CancellationToken cancellationToken = default)
        {
            if (amount <= 0)
            {
                return PaymentProviderValidationResult.Failure(
                    "Số tiền thanh toán không hợp lệ.",
                    "Invalid amount");
            }

            var normalizedCurrency = string.IsNullOrWhiteSpace(currency)
                ? "VND"
                : currency.Trim().ToUpperInvariant();

            var normalizedCode = string.IsNullOrWhiteSpace(paymentProviderCode)
                ? null
                : paymentProviderCode.Trim().ToUpperInvariant();

            var provider = string.IsNullOrWhiteSpace(normalizedCode)
                ? await _paymentProviderRepository.GetDefaultAvailableAsync(
                    normalizedCurrency,
                    amount,
                    cancellationToken)
                : await _paymentProviderRepository.GetActiveByCodeAsync(
                    normalizedCode,
                    cancellationToken);

            if (provider is null)
            {
                return PaymentProviderValidationResult.Failure(
                    "Phương thức thanh toán không khả dụng.",
                    "Payment provider not found");
            }

            var unavailableReason = provider.ValidateForCreatePayment(
                amount,
                normalizedCurrency);

            if (!string.IsNullOrWhiteSpace(unavailableReason))
            {
                return PaymentProviderValidationResult.Failure(
                    unavailableReason,
                    unavailableReason);
            }

            var configError = ValidateRuntimeConfig(provider.Code);

            if (!string.IsNullOrWhiteSpace(configError))
            {
                return PaymentProviderValidationResult.Failure(
                    configError,
                    configError);
            }

            return PaymentProviderValidationResult.Success(provider);
        }

        private string? ValidateRuntimeConfig(string providerCode)
        {
            return providerCode.ToUpperInvariant() switch
            {
                PaymentProviderCodes.Sepay => ValidateSepayOptions(),
                _ => $"Backend chưa hỗ trợ cổng thanh toán {providerCode}."
            };
        }

        private string? ValidateSepayOptions()
        {
            if (!_sepayOptions.Enabled)
                return "Cổng thanh toán SePay đang tạm tắt.";

            if (string.IsNullOrWhiteSpace(_sepayOptions.BankCode))
                return "Cấu hình SePay thiếu BankCode.";

            if (string.IsNullOrWhiteSpace(_sepayOptions.AccountNumber))
                return "Cấu hình SePay thiếu AccountNumber.";

            if (string.IsNullOrWhiteSpace(_sepayOptions.AccountName))
                return "Cấu hình SePay thiếu AccountName.";

            return null;
        }
    }
}
