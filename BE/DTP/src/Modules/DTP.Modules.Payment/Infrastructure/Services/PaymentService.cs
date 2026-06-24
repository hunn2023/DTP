using DTP.Modules.Delivery.Application.Commands.DeliverOrder;
using DTP.Modules.Delivery.Application.Commands.Delivery;
using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Payment.Application.Options;
using DTP.Modules.Payment.Domain.Entities;
using DTP.Modules.Payment.Domain.Enums;
using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentTransactionRepository _paymentRepository;
        private readonly IPaymentCallbackLogRepository _callbackLogRepository;
        private readonly IVnptEpayClient _vnptEpayClient;
        private readonly IOrderPaymentService _orderPaymentService;
        private readonly IPaymentAuditService _paymentAuditService;
        private readonly IPaymentUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IPaymentRateLimitService _paymentRateLimitService;
        private readonly IProviderReservationService _providerReservationService;
        private readonly IProviderFulfillmentService _providerFulfillmentService;

        public PaymentService(
            IPaymentTransactionRepository paymentRepository,
            IPaymentCallbackLogRepository callbackLogRepository,
            IVnptEpayClient vnptEpayClient,
            IOrderPaymentService orderPaymentService,
            IPaymentAuditService paymentAuditService,
            IPaymentUnitOfWork unitOfWork,
            IMediator mediator,
            IPaymentRateLimitService paymentRateLimitService,
            IProviderReservationService providerReservationService,
            IProviderFulfillmentService providerFulfillmentService)
        {
            _paymentRepository = paymentRepository;
            _callbackLogRepository = callbackLogRepository;
            _vnptEpayClient = vnptEpayClient;
            _orderPaymentService = orderPaymentService;
            _paymentAuditService = paymentAuditService;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _paymentRateLimitService = paymentRateLimitService;
            _providerReservationService = providerReservationService;
            _providerFulfillmentService = providerFulfillmentService;
        }


        public async Task<Result<PaymentTransactionDto>> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);

            if (payment == null)
                return Result<PaymentTransactionDto>.Failure("Không tìm thấy giao dịch thanh toán.");

            return Result<PaymentTransactionDto>.Success(Map(payment));
        }

        public async Task<Result<PaymentTransactionDto>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);

            if (payment == null)
                return Result<PaymentTransactionDto>.Failure("Không tìm thấy giao dịch thanh toán.");

            return Result<PaymentTransactionDto>.Success(Map(payment));
        }


        private static PaymentTransactionDto Map(PaymentTransaction payment)
        {
            return new PaymentTransactionDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                OrderCode = payment.OrderCode,
                CustomerId = payment.CustomerId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Provider = payment.Provider.ToString(),
                Method = payment.Method.ToString(),
                Status = payment.Status.ToString(),
                RequestId = payment.RequestId,
                ProviderTransactionId = payment.ProviderTransactionId,
                QrCode = payment.QrCode,
                QrImageUrl = payment.QrImageUrl,
                PaymentUrl = payment.PaymentUrl,
                ExpiredAt = payment.ExpiredAt,
                PaidAt = payment.PaidAt,
                CreatedAt = payment.CreatedAt
            };
        }


        private async Task CreateAndProcessDeliverySafeAsync(
            PaymentTransaction payment,
            string? ipAddress,
            CancellationToken cancellationToken)
        {
            try
            {
                var createDeliveryResult = await _mediator.Send(
                    new CreateDeliveryCommand(
                        payment.OrderId,
                        ipAddress),
                    cancellationToken);

                if (!createDeliveryResult.IsSuccess)
                {
                    await WritePaymentAuditSafeAsync(
                        action: "Payment Delivery Create Failed",
                        status: "Failed",
                        entityId: payment.Id,
                        description: "Payment was paid but delivery creation failed.",
                        newValues: new
                        {
                            payment.Id,
                            payment.OrderId,
                            payment.OrderCode,
                            Error = createDeliveryResult.Error,
                            IpAddress = ipAddress,
                            CreatedAt = DateTime.UtcNow
                        },
                        cancellationToken: cancellationToken);

                    return;
                }

                var processDeliveryResult = await _mediator.Send(
                    new ProcessDeliveryCommand(createDeliveryResult.Data, ipAddress),
                    cancellationToken);

                if (!processDeliveryResult.IsSuccess)
                {
                    await WritePaymentAuditSafeAsync(
                        action: "Payment Delivery Process Failed",
                        status: "Failed",
                        entityId: payment.Id,
                        description: "Payment was paid but delivery processing failed.",
                        newValues: new
                        {
                            payment.Id,
                            payment.OrderId,
                            payment.OrderCode,
                            DeliveryId = createDeliveryResult.Data,
                            Error = processDeliveryResult.Error,
                            IpAddress = ipAddress,
                            CreatedAt = DateTime.UtcNow
                        },
                        cancellationToken: cancellationToken);
                }
            }
            catch (Exception ex)
            {
                await WritePaymentAuditSafeAsync(
                    action: "Payment Delivery Exception",
                    status: "Failed",
                    entityId: payment.Id,
                    description: "Payment was paid but an exception occurred while creating or processing delivery.",
                    newValues: new
                    {
                        payment.Id,
                        payment.OrderId,
                        payment.OrderCode,
                        Error = ex.Message,
                        IpAddress = ipAddress,
                        CreatedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);
            }
        }

        private async Task WritePaymentAuditSafeAsync(
            string action,
            string status,
            Guid? entityId,
            string? description = null,
            object? oldValues = null,
            object? newValues = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _paymentAuditService.WriteAsync(
                    action: action,
                    status: status,
                    entityId: entityId,
                    description: description ?? "",
                    oldValues: oldValues,
                    newValues: newValues,
                    cancellationToken: cancellationToken);
            }
            catch
            {
                // Không để lỗi audit làm fail nghiệp vụ thanh toán.
            }
        }



        public async Task<Result<PaymentOrderStatusDto>> GetOrderPaymentStatusAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            if (orderId == Guid.Empty)
                return Result<PaymentOrderStatusDto>.Failure("OrderId không hợp lệ.");

            var payment = await _paymentRepository.GetLatestByOrderIdAsync(
                orderId,
                cancellationToken);

            if (payment == null)
            {
                return Result<PaymentOrderStatusDto>.Success(new PaymentOrderStatusDto
                {
                    OrderId = orderId,
                    Status = "NotCreated",
                    IsPaid = false,
                    IsExpired = false,
                    Message = "Chưa có giao dịch thanh toán."
                });
            }

            var now = DateTime.UtcNow;

            var isPaid = payment.Status == PaymentStatus.Paid;

            var isExpired =
                payment.Status == PaymentStatus.Expired ||
                (
                    payment.Status == PaymentStatus.Pending &&
                    payment.ExpiredAt.HasValue &&
                    payment.ExpiredAt.Value <= now
                );

            var status = payment.Status.ToString();

            if (isExpired && payment.Status == PaymentStatus.Pending)
            {
                status = PaymentStatus.Expired.ToString();
            }

            var message = status switch
            {
                "Paid" => "Thanh toán thành công.",
                "Pending" => "Đang chờ thanh toán.",
                "Expired" => "Giao dịch thanh toán đã hết hạn.",
                "Failed" => "Giao dịch thanh toán thất bại.",
                _ => "Đang xử lý trạng thái thanh toán."
            };

            return Result<PaymentOrderStatusDto>.Success(new PaymentOrderStatusDto
            {
                OrderId = payment.OrderId,
                OrderCode = payment.OrderCode,
                PaymentTransactionId = payment.Id,
                Status = status,
                Provider = payment.Provider.ToString(),
                Amount = payment.Amount,
                Currency = payment.Currency,
                IsPaid = isPaid,
                IsExpired = isExpired,
                PaidAt = payment.PaidAt,
                ExpiredAt = payment.ExpiredAt,
                Message = message
            });
        }
    }
}