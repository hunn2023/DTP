using DTP.Modules.Delivery.Application.Commands.DeliverOrder;
using DTP.Modules.Delivery.Application.Commands.Delivery;
using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Payment.Domain.Entities;
using DTP.Modules.Payment.Domain.Enums;
using DTP.Shared.Application;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        public PaymentService(
            IPaymentTransactionRepository paymentRepository,
            IPaymentCallbackLogRepository callbackLogRepository,
            IVnptEpayClient vnptEpayClient,
            IOrderPaymentService orderPaymentService,
            IPaymentAuditService paymentAuditService,
            IPaymentUnitOfWork unitOfWork,
            IMediator mediator)
        {
            _paymentRepository = paymentRepository;
            _callbackLogRepository = callbackLogRepository;
            _vnptEpayClient = vnptEpayClient;
            _orderPaymentService = orderPaymentService;
            _paymentAuditService = paymentAuditService;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }


        public async Task<Result<PaymentQrResponseDto>> CreateQrAsync(
    Guid orderId,
    string ipAddress,
    CancellationToken cancellationToken = default)
        {
            if (orderId == Guid.Empty)
                return Result<PaymentQrResponseDto>.Failure("OrderId không hợp lệ.");

            var order = await _orderPaymentService.GetOrderPaymentInfoAsync(
                orderId,
                cancellationToken);

            if (order == null)
                return Result<PaymentQrResponseDto>.Failure("Không tìm thấy đơn hàng.");

            if (order.TotalAmount <= 0)
                return Result<PaymentQrResponseDto>.Failure("Số tiền thanh toán không hợp lệ.");

            var hasPaid = await _paymentRepository.HasPaidPaymentByOrderIdAsync(
                orderId,
                cancellationToken);

            if (hasPaid)
                return Result<PaymentQrResponseDto>.Failure("Đơn hàng đã được thanh toán.");

            var existingPending = await _paymentRepository.GetPendingByOrderIdAsync(
                orderId,
                cancellationToken);

            if (existingPending != null)
            {
                var hasQr = HasQrData(existingPending);

                if (existingPending.ExpiredAt.HasValue &&
                    existingPending.ExpiredAt.Value > DateTime.UtcNow &&
                    hasQr)
                {
                    await _paymentAuditService.WriteAsync(
                        action: "Payment QR Reused",
                        status: "Success",
                        entityId: existingPending.Id,
                        description: "Reuse existing pending payment QR.",
                        oldValues: null,
                        newValues: new
                        {
                            existingPending.Id,
                            existingPending.OrderId,
                            existingPending.OrderCode,
                            existingPending.Amount,
                            existingPending.Currency,
                            existingPending.QrImageUrl,
                            existingPending.PaymentUrl,
                            existingPending.ExpiredAt,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Result<PaymentQrResponseDto>.Success(MapQr(existingPending));
                }

                if (!hasQr)
                {
                    return Result<PaymentQrResponseDto>.Failure(
                        "Mã QR đang được tạo, vui lòng thử lại sau.");
                }

                existingPending.MarkExpired();

                _paymentRepository.Update(existingPending);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _paymentAuditService.WriteAsync(
                    action: "Payment QR Expired",
                    status: "Success",
                    entityId: existingPending.Id,
                    description: "Existing payment QR expired before creating new QR.",
                    oldValues: null,
                    newValues: new
                    {
                        existingPending.Id,
                        existingPending.OrderId,
                        existingPending.OrderCode,
                        existingPending.ExpiredAt,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);
            }

            var requestId = $"DTP-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8]}";

            var payment = new PaymentTransaction(
                orderId: order.OrderId,
                orderCode: order.OrderCode,
                customerId: order.CustomerId,
                amount: order.TotalAmount,
                currency: order.Currency,
                provider: PaymentProvider.VnptEpay,
                method: PaymentMethod.BankTransferQr,
                requestId: requestId,
                ipAddress: ipAddress);

            try
            {
                await _paymentRepository.AddAsync(payment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                var concurrentPending = await _paymentRepository.GetPendingByOrderIdAsync(
                    orderId,
                    cancellationToken);

                if (concurrentPending != null)
                {
                    if (HasQrData(concurrentPending))
                    {
                        await _paymentAuditService.WriteAsync(
                            action: "Payment QR Reused After Concurrent Request",
                            status: "Success",
                            entityId: concurrentPending.Id,
                            description: "Reuse existing QR after concurrent create request.",
                            oldValues: null,
                            newValues: new
                            {
                                concurrentPending.Id,
                                concurrentPending.OrderId,
                                concurrentPending.OrderCode,
                                concurrentPending.Amount,
                                concurrentPending.Currency,
                                concurrentPending.QrImageUrl,
                                concurrentPending.PaymentUrl,
                                concurrentPending.ExpiredAt,
                                IpAddress = ipAddress
                            },
                            cancellationToken: cancellationToken);

                        return Result<PaymentQrResponseDto>.Success(MapQr(concurrentPending));
                    }

                    return Result<PaymentQrResponseDto>.Failure(
                        "Mã QR đang được tạo, vui lòng thử lại sau.");
                }

                throw;
            }

            await _paymentAuditService.WriteAsync(
                action: "Payment QR Create Requested",
                status: "Success",
                entityId: payment.Id,
                description: "Payment QR create request initialized.",
                oldValues: null,
                newValues: new
                {
                    payment.Id,
                    payment.OrderId,
                    payment.OrderCode,
                    payment.Amount,
                    payment.Currency,
                    payment.RequestId,
                    Provider = payment.Provider.ToString(),
                    Method = payment.Method.ToString(),
                    IpAddress = ipAddress
                },
                cancellationToken: cancellationToken);

            var vnptRequest = new VnptEpayCreateQrRequest
            {
                RequestId = payment.RequestId,
                OrderCode = payment.OrderCode,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Description = $"Thanh toan don hang {payment.OrderCode}"
            };

            VnptEpayCreateQrResponse vnptResponse;

            try
            {
                vnptResponse = await _vnptEpayClient.CreateQrAsync(
                    vnptRequest,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                payment.MarkCreateQrFailed(
                    providerResponseCode: "EXCEPTION",
                    providerResponseMessage: ex.Message,
                    rawProviderResponse: ex.ToString());

                _paymentRepository.Update(payment);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _paymentAuditService.WriteAsync(
                    action: "Payment QR Create Exception",
                    status: "Failed",
                    entityId: payment.Id,
                    description: "Exception occurred while creating VNPT ePay QR.",
                    oldValues: null,
                    newValues: new
                    {
                        payment.Id,
                        payment.OrderId,
                        payment.OrderCode,
                        payment.Amount,
                        payment.Currency,
                        payment.RequestId,
                        Error = ex.Message,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Result<PaymentQrResponseDto>.Failure(
                    "Không thể kết nối VNPT ePay để tạo mã QR.");
            }

            if (!vnptResponse.IsSuccess)
            {
                payment.MarkCreateQrFailed(
                    providerResponseCode: vnptResponse.ResponseCode,
                    providerResponseMessage: vnptResponse.Message,
                    rawProviderResponse: vnptResponse.RawResponse);

                _paymentRepository.Update(payment);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _paymentAuditService.WriteAsync(
                    action: "Payment QR Create Failed",
                    status: "Failed",
                    entityId: payment.Id,
                    description: "VNPT ePay failed to create QR.",
                    oldValues: null,
                    newValues: new
                    {
                        payment.Id,
                        payment.OrderId,
                        payment.OrderCode,
                        payment.Amount,
                        payment.Currency,
                        payment.RequestId,
                        vnptResponse.ResponseCode,
                        vnptResponse.Message,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Result<PaymentQrResponseDto>.Failure(
                    vnptResponse.Message ?? "Không tạo được mã QR thanh toán.");
            }

            payment.MarkQrCreated(
                providerTransactionId: vnptResponse.ProviderTransactionId,
                providerPaymentCode: vnptResponse.ProviderPaymentCode,
                qrCode: vnptResponse.QrCode,
                qrImageUrl: vnptResponse.QrImageUrl,
                paymentUrl: vnptResponse.PaymentUrl,
                expiredAt: vnptResponse.ExpiredAt,
                bankCode: vnptResponse.BankCode,
                bankAccountNo: vnptResponse.BankAccountNo,
                bankAccountName: vnptResponse.BankAccountName,
                transferContent: vnptResponse.TransferContent,
                providerResponseCode: vnptResponse.ResponseCode,
                providerResponseMessage: vnptResponse.Message,
                rawProviderRequest: vnptResponse.RawRequest,
                rawProviderResponse: vnptResponse.RawResponse);

            _paymentRepository.Update(payment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _paymentAuditService.WriteAsync(
                action: "Payment QR Created",
                status: "Success",
                entityId: payment.Id,
                description: "VNPT ePay QR created successfully.",
                oldValues: null,
                newValues: new
                {
                    payment.Id,
                    payment.OrderId,
                    payment.OrderCode,
                    payment.Amount,
                    payment.Currency,
                    payment.RequestId,
                    payment.ProviderTransactionId,
                    payment.ProviderPaymentCode,
                    payment.QrCode,
                    payment.QrImageUrl,
                    payment.PaymentUrl,
                    payment.BankCode,
                    payment.BankAccountNo,
                    payment.BankAccountName,
                    payment.TransferContent,
                    payment.ExpiredAt,
                    IpAddress = ipAddress
                },
                cancellationToken: cancellationToken);

            return Result<PaymentQrResponseDto>.Success(MapQr(payment));
        }


        private static bool HasQrData(PaymentTransaction payment)
        {
            return !string.IsNullOrWhiteSpace(payment.QrCode) ||
                   !string.IsNullOrWhiteSpace(payment.QrImageUrl) ||
                   !string.IsNullOrWhiteSpace(payment.PaymentUrl);
        }

        public async Task<Result> HandleVnptEpayCallbackAsync(
            VnptEpayCallbackDto callback,
            string rawBody,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var callbackLog = new PaymentCallbackLog(
                provider: PaymentProvider.VnptEpay,
                requestId: callback.RequestId,
                providerTransactionId: callback.ProviderTransactionId,
                rawBody: rawBody,
                signature: callback.Signature,
                ipAddress: ipAddress,
                status: PaymentCallbackStatus.Received);

            await _callbackLogRepository.AddAsync(callbackLog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var isValidSignature = _vnptEpayClient.VerifyCallbackSignature(callback);

            if (!isValidSignature)
            {
                callbackLog.MarkInvalidSignature("VNPT ePay callback signature is invalid.");
                _callbackLogRepository.Update(callbackLog);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _paymentAuditService.WriteAsync(
                    action: "Payment Callback Invalid Signature",
                    status: "Failed",
                    entityId: callbackLog.Id,
                    description: "VNPT ePay callback signature validation failed.",
                    oldValues: null,
                    newValues: new
                    {
                        callback.RequestId,
                        callback.OrderCode,
                        callback.ProviderTransactionId,
                        callback.Amount,
                        callback.Currency,
                        callback.Status,
                        callback.ResponseCode,
                        callback.Message,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Invalid signature.");
            }

            callbackLog.MarkVerified();

            var alreadyProcessed = await _callbackLogRepository.ExistsProcessedAsync(
                callback.RequestId,
                callback.ProviderTransactionId,
                cancellationToken);

            if (alreadyProcessed)
            {
                callbackLog.MarkDuplicated("Callback already processed.");
                _callbackLogRepository.Update(callbackLog);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _paymentAuditService.WriteAsync(
                    action: "Payment Callback Duplicated",
                    status: "Success",
                    entityId: callbackLog.Id,
                    description: "Duplicated VNPT ePay callback ignored.",
                    oldValues: null,
                    newValues: new
                    {
                        callback.RequestId,
                        callback.OrderCode,
                        callback.ProviderTransactionId,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Result.Success();
            }

            PaymentTransaction? payment = null;

            if (!string.IsNullOrWhiteSpace(callback.RequestId))
            {
                payment = await _paymentRepository.GetByRequestIdAsync(
                    callback.RequestId,
                    cancellationToken);
            }

            if (payment == null && !string.IsNullOrWhiteSpace(callback.ProviderTransactionId))
            {
                payment = await _paymentRepository.GetByProviderTransactionIdAsync(
                    PaymentProvider.VnptEpay,
                    callback.ProviderTransactionId,
                    cancellationToken);
            }

            if (payment == null)
            {
                callbackLog.MarkFailed("Payment transaction not found.");
                _callbackLogRepository.Update(callbackLog);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _paymentAuditService.WriteAsync(
                    action: "Payment Callback Payment Not Found",
                    status: "Failed",
                    entityId: callbackLog.Id,
                    description: "Cannot find payment transaction for VNPT ePay callback.",
                    oldValues: null,
                    newValues: new
                    {
                        callback.RequestId,
                        callback.OrderCode,
                        callback.ProviderTransactionId,
                        callback.Amount,
                        callback.Currency,
                        callback.Status,
                        callback.ResponseCode,
                        callback.Message,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Payment transaction not found.");
            }

            callbackLog.AttachPayment(payment.Id);

            if (payment.IsPaid())
            {
                callbackLog.MarkDuplicated("Payment already paid.");
                _callbackLogRepository.Update(callbackLog);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            if (payment.Amount != callback.Amount)
            {
                payment.MarkFailed(
                    callback.ResponseCode,
                    "Callback amount does not match payment amount.",
                    rawBody);

                callbackLog.MarkFailed("Callback amount does not match payment amount.");

                _paymentRepository.Update(payment);
                _callbackLogRepository.Update(callbackLog);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _paymentAuditService.WriteAsync(
                    action: "Payment Callback Amount Mismatch",
                    status: "Failed",
                    entityId: payment.Id,
                    description: "VNPT ePay callback amount does not match payment amount.",
                    oldValues: null,
                    newValues: new
                    {
                        payment.Id,
                        payment.OrderId,
                        payment.OrderCode,
                        PaymentAmount = payment.Amount,
                        CallbackAmount = callback.Amount,
                        callback.RequestId,
                        callback.ProviderTransactionId,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Amount mismatch.");
            }

            var isPaidCallback =
                callback.Status == "PAID" ||
                callback.Status == "SUCCESS" ||
                callback.ResponseCode == "00";

            if (!isPaidCallback)
            {
                payment.MarkFailed(
                    callback.ResponseCode,
                    callback.Message,
                    rawBody);

                callbackLog.MarkProcessed();

                _paymentRepository.Update(payment);
                _callbackLogRepository.Update(callbackLog);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _paymentAuditService.WriteAsync(
                    action: "Payment Callback Failed",
                    status: "Failed",
                    entityId: payment.Id,
                    description: "VNPT ePay callback returned failed payment status.",
                    oldValues: null,
                    newValues: new
                    {
                        payment.Id,
                        payment.OrderId,
                        payment.OrderCode,
                        payment.Amount,
                        callback.RequestId,
                        callback.ProviderTransactionId,
                        callback.Status,
                        callback.ResponseCode,
                        callback.Message,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Result.Success();
            }

            payment.MarkPaid(
                callback.ProviderTransactionId,
                callback.ResponseCode,
                callback.Message,
                rawBody);

            callbackLog.MarkProcessed();

            _paymentRepository.Update(payment);
            _callbackLogRepository.Update(callbackLog);

            await _orderPaymentService.MarkOrderPaidAsync(
                payment.OrderId,
                payment.Id,
                payment.ProviderTransactionId,
                payment.PaidAt ?? DateTime.UtcNow,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _paymentAuditService.WriteAsync(
               action: "Payment Callback Success",
               status: "Success",
               entityId: payment.Id,
               description: "Payment marked as paid from VNPT ePay callback.",
               oldValues: null,
               newValues: new
               {
                   payment.Id,
                   payment.OrderId,
                   payment.OrderCode,
                   payment.Amount,
                   payment.Currency,
                   payment.RequestId,
                   payment.ProviderTransactionId,
                   payment.PaidAt,
                   callback.ResponseCode,
                   callback.Message,
                   IpAddress = ipAddress
               },
               cancellationToken: cancellationToken);


            //Gửi email

            try
            {
                var createDeliveryResult = await _mediator.Send(
                    new CreateDeliveryCommand(
                        payment.OrderId,
                        ipAddress),
                    cancellationToken);

                if (createDeliveryResult.IsSuccess)
                {
                    var processDeliveryResult = await _mediator.Send(
                        new ProcessDeliveryCommand(createDeliveryResult.Data),
                        cancellationToken);

                    if (!processDeliveryResult.IsSuccess)
                    {
                        await _paymentAuditService.WriteAsync(
                            action: "Payment Delivery Process Failed",
                            status: "Failed",
                            entityId: payment.Id,
                            description: "Payment was paid but delivery processing failed.",
                            oldValues: null,
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
                else
                {
                    await _paymentAuditService.WriteAsync(
                        action: "Payment Delivery Create Failed",
                        status: "Failed",
                        entityId: payment.Id,
                        description: "Payment was paid but delivery creation failed.",
                        oldValues: null,
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
                }
            }
            catch (Exception ex)
            {
                await _paymentAuditService.WriteAsync(
                    action: "Payment Delivery Exception",
                    status: "Failed",
                    entityId: payment.Id,
                    description: "Payment was paid but an exception occurred while creating or processing delivery.",
                    oldValues: null,
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


            return Result.Success();
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

        private static PaymentQrResponseDto MapQr(PaymentTransaction payment)
        {
            return new PaymentQrResponseDto
            {
                PaymentTransactionId = payment.Id,
                OrderId = payment.OrderId,
                OrderCode = payment.OrderCode,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status.ToString(),
                ProviderTransactionId = payment.ProviderTransactionId,
                QrCode = payment.QrCode,
                QrImageUrl = payment.QrImageUrl,
                PaymentUrl = payment.PaymentUrl,
                BankCode = payment.BankCode,
                BankAccountNo = payment.BankAccountNo,
                BankAccountName = payment.BankAccountName,
                TransferContent = payment.TransferContent,
                ExpiredAt = payment.ExpiredAt
            };
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
    }
}
