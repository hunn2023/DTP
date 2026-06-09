using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Payment.Domain.Entities;
using DTP.Modules.Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentTransactionRepository _transactionRepository;
        private readonly IPaymentUnitOfWork _unitOfWork;
        private readonly IVnptEpayClient _vnptEpayClient;
        private readonly IPaymentOrderingService _orderingService;

        public PaymentService(
            IPaymentTransactionRepository transactionRepository,
            IPaymentUnitOfWork unitOfWork,
            IVnptEpayClient vnptEpayClient,
            IPaymentOrderingService orderingService)
        {
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _vnptEpayClient = vnptEpayClient;
            _orderingService = orderingService;
        }

        public async Task<CreatePaymentResultDto> CreatePaymentAsync(
            CreatePaymentDto request,
            CancellationToken cancellationToken = default)
        {
            if (request.OrderId == Guid.Empty)
                throw new Exception("OrderId is required.");

            if (string.IsNullOrWhiteSpace(request.OrderCode))
                throw new Exception("OrderCode is required.");

            if (request.Amount <= 0)
                throw new Exception("Amount must be greater than zero.");

            var existing = await _transactionRepository.GetByOrderIdAsync(
                request.OrderId,
                cancellationToken);

            if (existing != null && existing.Status == PaymentTransactionStatus.Pending)
            {
                return MapToCreatePaymentResult(existing);
            }

            var transactionCode = GenerateTransactionCode();

            var transaction = new PaymentTransaction(
                request.OrderId,
                request.OrderCode,
                PaymentProviderCode.VNPT_EPAY,
                transactionCode,
                request.Amount,
                request.CurrencyCode);

            await _transactionRepository.AddAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var vnptRequest = new VnptEpayCreateQrRequest
            {
                TransactionCode = transaction.TransactionCode,
                OrderCode = transaction.OrderCode,
                Amount = transaction.Amount,
                CurrencyCode = transaction.CurrencyCode,
                CustomerEmail = request.CustomerEmail,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                ReturnUrl = request.ReturnUrl,
                CallbackUrl = request.CallbackUrl
            };

            var providerResult = await _vnptEpayClient.CreateQrPaymentAsync(
                vnptRequest,
                cancellationToken);

            if (!providerResult.Success)
            {
                transaction.MarkFailed(providerResult.ErrorMessage ?? "Create VNPT ePay QR failed.");

                _transactionRepository.Update(transaction);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new Exception(providerResult.ErrorMessage ?? "Create payment failed.");
            }

            transaction.AttachProviderResult(
                providerResult.ProviderTransactionCode,
                providerResult.PaymentUrl,
                providerResult.QrCodeUrl,
                providerResult.QrContent,
                providerResult.ExpiredAt,
                providerResult.RawRequest,
                providerResult.RawResponse);

            _transactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToCreatePaymentResult(transaction);
        }

        public async Task<bool> HandleVnptEpayCallbackAsync(
            VnptEpayCallbackDto request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.RawBody))
                throw new Exception("Callback raw body is required.");

            var isValidSignature = _vnptEpayClient.VerifyCallbackSignature(request);

            PaymentTransaction? transaction = null;

            if (!string.IsNullOrWhiteSpace(request.TransactionCode))
            {
                transaction = await _transactionRepository.GetByTransactionCodeAsync(
                    request.TransactionCode,
                    cancellationToken);
            }

            if (transaction == null && !string.IsNullOrWhiteSpace(request.OrderCode))
            {
                transaction = await _transactionRepository.GetByOrderCodeAsync(
                    request.OrderCode,
                    cancellationToken);
            }

            var callback = new PaymentCallback(
                transaction?.Id,
                PaymentProviderCode.VNPT_EPAY,
                request.OrderCode,
                request.TransactionCode,
                request.ProviderTransactionCode,
                request.RawBody,
                request.Signature,
                isValidSignature);

            await _transactionRepository.AddCallbackAsync(callback, cancellationToken);

            if (!isValidSignature)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return false;
            }

            if (transaction == null)
            {
                callback.MarkFailed("Payment transaction not found.");
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return false;
            }

            if (transaction.Status == PaymentTransactionStatus.Success)
            {
                callback.MarkIgnored("Transaction already processed.");
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }

            var isSuccess = IsVnptPaymentSuccess(request.Status);

            if (isSuccess)
            {
                transaction.MarkSuccess(request.ProviderTransactionCode);

                _transactionRepository.Update(transaction);

                await _orderingService.MarkOrderPaidAsync(
                    transaction.OrderId,
                    "VNPT ePay payment success.",
                    cancellationToken);

                callback.MarkProcessed();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return true;
            }

            transaction.MarkFailed(request.Message ?? "VNPT ePay payment failed.");

            _transactionRepository.Update(transaction);

            await _orderingService.MarkOrderPaymentFailedAsync(
                transaction.OrderId,
                request.Message ?? "VNPT ePay payment failed.",
                cancellationToken);

            callback.MarkProcessed();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<PaymentTransactionDto?> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var transaction = await _transactionRepository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

            if (transaction == null)
                return null;

            return new PaymentTransactionDto
            {
                Id = transaction.Id,
                OrderId = transaction.OrderId,
                OrderCode = transaction.OrderCode,
                TransactionCode = transaction.TransactionCode,
                ProviderTransactionCode = transaction.ProviderTransactionCode,
                Amount = transaction.Amount,
                CurrencyCode = transaction.CurrencyCode,
                Status = transaction.Status,
                StatusName = transaction.Status.ToString(),
                PaymentUrl = transaction.PaymentUrl,
                QrCodeUrl = transaction.QrCodeUrl,
                QrContent = transaction.QrContent,
                ExpiredAt = transaction.ExpiredAt,
                PaidAt = transaction.PaidAt,
                CreatedAt = transaction.CreatedAt
            };
        }

        private static string GenerateTransactionCode()
        {
            return $"PAY{DateTime.UtcNow:yyyyMMddHHmmssfff}{Random.Shared.Next(1000, 9999)}";
        }

        private static bool IsVnptPaymentSuccess(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return false;

            return status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase)
                   || status.Equals("PAID", StringComparison.OrdinalIgnoreCase)
                   || status == "00";
        }

        private static CreatePaymentResultDto MapToCreatePaymentResult(
            PaymentTransaction transaction)
        {
            return new CreatePaymentResultDto
            {
                PaymentTransactionId = transaction.Id,
                PaymentTransactionCode = transaction.TransactionCode,
                ProviderTransactionCode = transaction.ProviderTransactionCode,
                PaymentUrl = transaction.PaymentUrl,
                QrCodeUrl = transaction.QrCodeUrl,
                QrContent = transaction.QrContent,
                ExpiredAt = transaction.ExpiredAt
            };
        }
    }
}
