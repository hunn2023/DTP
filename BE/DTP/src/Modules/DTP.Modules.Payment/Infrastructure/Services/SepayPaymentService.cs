using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Payment.Application.Options;
using DTP.Modules.Payment.Domain.Entities;
using DTP.Modules.Payment.Domain.Enums;
using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public class SepayPaymentService : ISepayPaymentService
    {
        private readonly IPaymentTransactionRepository _paymentRepository;
        private readonly IPaymentCallbackLogRepository _callbackLogRepository;
        private readonly IOrderPaymentService _orderPaymentService;
        private readonly IPaymentAuditService _paymentAuditService;
        private readonly IPaymentProviderService _paymentProviderService;
        private readonly IPaymentUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IPaymentRateLimitService _paymentRateLimitService;
        private readonly IProviderReservationService _providerReservationService;
        private readonly IProviderFulfillmentService _providerFulfillmentService;
        private readonly SepayOptions _sepayOptions;
        private readonly IPaymentRealtimeNotifier _paymentRealtimeNotifier;
        private readonly IPaymentProviderRepository _paymentProviderRepository;
        public SepayPaymentService(
            IPaymentTransactionRepository paymentRepository,
            IPaymentCallbackLogRepository callbackLogRepository,
            IOrderPaymentService orderPaymentService,
            IPaymentAuditService paymentAuditService,
            IPaymentUnitOfWork unitOfWork,
            IMediator mediator,
            IPaymentRateLimitService paymentRateLimitService,
            IProviderReservationService providerReservationService,
            IProviderFulfillmentService providerFulfillmentService,
             IOptions<SepayOptions> sepayOptions,
             IPaymentRealtimeNotifier paymentRealtimeNotifier,
             IPaymentProviderRepository paymentProviderRepository,
             IPaymentProviderService paymentProviderService
             )
        {
            _paymentRepository = paymentRepository;
            _callbackLogRepository = callbackLogRepository;
            _orderPaymentService = orderPaymentService;
            _sepayOptions = sepayOptions.Value;
            _paymentRealtimeNotifier = paymentRealtimeNotifier;
            _paymentAuditService = paymentAuditService;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _paymentRateLimitService = paymentRateLimitService;
            _providerReservationService = providerReservationService;
            _providerFulfillmentService = providerFulfillmentService;
            _paymentProviderRepository = paymentProviderRepository;
            _paymentProviderService = paymentProviderService;
        }


        #region sepay
        public async Task<Result<PaymentQrResponseDto>> CreateQrAsync(
           Guid orderId,
           string paymentProviderCode,
           string ipAddress,
           CancellationToken cancellationToken = default)
        {
            if (orderId == Guid.Empty)
                return Result<PaymentQrResponseDto>.Failure("OrderId không hợp lệ.");

            ipAddress = string.IsNullOrWhiteSpace(ipAddress)
                ? "unknown"
                : ipAddress.Trim();

            var order = await _orderPaymentService.GetOrderPaymentInfoAsync(
                orderId,
                cancellationToken);

            if (order == null)
            {
                await WritePaymentAuditSafeAsync(
                    action: "Payment QR Create Failed",
                    status: "Failed",
                    entityId: null,
                    description: "Create payment QR failed because order was not found.",
                    newValues: new
                    {
                        OrderId = orderId,
                        IpAddress = ipAddress,
                        Reason = "Order not found"
                    },
                    cancellationToken: cancellationToken);

                return Result<PaymentQrResponseDto>.Failure("Không tìm thấy đơn hàng.");
            }

            var createQrBlocked = await _paymentRateLimitService.IsCreateQrBlockedAsync(
                orderId,
                order.CustomerId,
                ipAddress,
                cancellationToken);

            if (createQrBlocked)
            {
                await WritePaymentAuditSafeAsync(
                    action: "Payment QR Create Blocked",
                    status: "Failed",
                    entityId: null,
                    description: "Create payment QR was blocked by rate limit.",
                    newValues: new
                    {
                        OrderId = orderId,
                        order.OrderCode,
                        order.CustomerId,
                        IpAddress = ipAddress,
                        Reason = "Create QR rate limit exceeded"
                    },
                    cancellationToken: cancellationToken);

                return Result<PaymentQrResponseDto>.Failure(
                    "Bạn tạo mã QR quá nhiều lần. Vui lòng thử lại sau.");
            }

            await _paymentRateLimitService.RegisterCreateQrAttemptAsync(
                orderId,
                order.CustomerId,
                ipAddress,
                cancellationToken);

            //if (!_sepayOptions.Enabled)
            //{
            //    await WritePaymentAuditSafeAsync(
            //        action: "Payment QR Create Failed",
            //        status: "Failed",
            //        entityId: null,
            //        description: "Create payment QR failed because SePay is disabled.",
            //        newValues: new
            //        {
            //            OrderId = orderId,
            //            order.OrderCode,
            //            IpAddress = ipAddress,
            //            Reason = "SePay disabled"
            //        },
            //        cancellationToken: cancellationToken);

            //    return Result<PaymentQrResponseDto>.Failure(
            //        "Cổng thanh toán SePay đang tạm tắt.");
            //}

            //var sepayConfigError = ValidateSepayOptions();

            //if (!string.IsNullOrWhiteSpace(sepayConfigError))
            //{
            //    await WritePaymentAuditSafeAsync(
            //        action: "Payment QR Create Failed",
            //        status: "Failed",
            //        entityId: null,
            //        description: "Create payment QR failed because SePay config is invalid.",
            //        newValues: new
            //        {
            //            OrderId = orderId,
            //            order.OrderCode,
            //            IpAddress = ipAddress,
            //            Reason = sepayConfigError
            //        },
            //        cancellationToken: cancellationToken);

            //    return Result<PaymentQrResponseDto>.Failure(
            //        "Cấu hình SePay chưa hợp lệ.");
            //}

            if (order.TotalAmount <= 0)
            {
                await WritePaymentAuditSafeAsync(
                    action: "Payment QR Create Failed",
                    status: "Failed",
                    entityId: null,
                    description: "Create payment QR failed because amount is invalid.",
                    newValues: new
                    {
                        OrderId = orderId,
                        order.OrderCode,
                        order.TotalAmount,
                        order.Currency,
                        IpAddress = ipAddress,
                        Reason = "Invalid amount"
                    },
                    cancellationToken: cancellationToken);

                return Result<PaymentQrResponseDto>.Failure(
                    "Số tiền thanh toán không hợp lệ.");
            }

            //if (!string.Equals(order.Currency, "VND", StringComparison.OrdinalIgnoreCase))
            //{
            //    await WritePaymentAuditSafeAsync(
            //        action: "Payment QR Create Failed",
            //        status: "Failed",
            //        entityId: null,
            //        description: "Create payment QR failed because SePay only supports VND in this flow.",
            //        newValues: new
            //        {
            //            OrderId = orderId,
            //            order.OrderCode,
            //            order.TotalAmount,
            //            order.Currency,
            //            IpAddress = ipAddress,
            //            Reason = "Invalid currency"
            //        },
            //        cancellationToken: cancellationToken);

            //    return Result<PaymentQrResponseDto>.Failure(
            //        "SePay chỉ hỗ trợ thanh toán VND.");
            //}

            if (order.PaymentExpiredAt.HasValue &&
                order.PaymentExpiredAt.Value < DateTime.UtcNow)
            {
                await WritePaymentAuditSafeAsync(
                    action: "Payment QR Create Failed",
                    status: "Failed",
                    entityId: null,
                    description: "Create payment QR failed because order payment time expired.",
                    newValues: new
                    {
                        OrderId = orderId,
                        order.OrderCode,
                        order.PaymentExpiredAt,
                        IpAddress = ipAddress,
                        Reason = "Order payment expired"
                    },
                    cancellationToken: cancellationToken);

                return Result<PaymentQrResponseDto>.Failure(
                    "Đơn hàng đã hết hạn thanh toán. Vui lòng tạo đơn hàng mới.");
            }

            var hasPaid = await _paymentRepository.HasPaidPaymentByOrderIdAsync(
                orderId,
                cancellationToken);

            if (hasPaid)
            {
                await WritePaymentAuditSafeAsync(
                    action: "Payment QR Create Failed",
                    status: "Failed",
                    entityId: null,
                    description: "Create payment QR failed because order has already been paid.",
                    newValues: new
                    {
                        OrderId = orderId,
                        order.OrderCode,
                        IpAddress = ipAddress,
                        Reason = "Order already paid"
                    },
                    cancellationToken: cancellationToken);

                return Result<PaymentQrResponseDto>.Failure(
                    "Đơn hàng đã được thanh toán.");
            }



            var paymentProviderValidation = await _paymentProviderService.ValidateForCreateQrAsync(
                    paymentProviderCode,
                    decimal.Round(order.TotalAmount, 0),
                    order.Currency,
                    cancellationToken);


            if (!paymentProviderValidation.IsValid)
            {
                await WritePaymentAuditSafeAsync(
                    action: "Payment Provider Validate Failed",
                    status: "Failed",
                    entityId: null,
                    description: "Create payment QR failed because payment provider is not available.",
                    newValues: new
                    {
                        OrderId = orderId,
                        order.OrderCode,
                        order.TotalAmount,
                        order.Currency,
                        RequestedPaymentProviderCode = paymentProviderCode,
                        Reason = paymentProviderValidation.Reason,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Result<PaymentQrResponseDto>.Failure(
                    paymentProviderValidation.ErrorMessage ?? "Phương thức thanh toán không khả dụng.");
            }

            var paymentProvider = paymentProviderValidation.Provider!;


            var existingPending = await _paymentRepository.GetPendingByOrderIdAsync(
                orderId,
                cancellationToken);

            if (existingPending != null)
            {
                var hasQr = HasQrData(existingPending);

                var qrStillValid =
                    existingPending.ExpiredAt.HasValue &&
                    existingPending.ExpiredAt.Value > DateTime.UtcNow;

                if (hasQr && qrStillValid)
                {
                    var reservationValid = await _providerReservationService.IsReservationValidAsync(
                        orderId,
                        cancellationToken);

                    if (reservationValid)
                    {
                        await WritePaymentAuditSafeAsync(
                            action: "Payment QR Reused",
                            status: "Success",
                            entityId: existingPending.Id,
                            description: "Reuse existing pending SePay payment QR.",
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
                                existingPending.BankCode,
                                existingPending.BankAccountNo,
                                existingPending.BankAccountName,
                                existingPending.TransferContent,
                                ProviderReservation = "Valid",
                                IpAddress = ipAddress
                            },
                            cancellationToken: cancellationToken);

                        return Result<PaymentQrResponseDto>.Success(MapQr(existingPending));
                    }

                    existingPending.MarkExpired();
                    _paymentRepository.Update(existingPending);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                if (!hasQr && qrStillValid)
                {
                    await WritePaymentAuditSafeAsync(
                        action: "Payment QR Creating",
                        status: "Failed",
                        entityId: existingPending.Id,
                        description: "Payment QR is being created by another request.",
                        newValues: new
                        {
                            existingPending.Id,
                            existingPending.OrderId,
                            existingPending.OrderCode,
                            existingPending.ExpiredAt,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Result<PaymentQrResponseDto>.Failure(
                        "Mã QR đang được tạo, vui lòng thử lại sau.");
                }

                existingPending.MarkExpired();

                _paymentRepository.Update(existingPending);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await WritePaymentAuditSafeAsync(
                    action: "Payment QR Expired",
                    status: "Success",
                    entityId: existingPending.Id,
                    description: "Existing payment QR expired before creating new SePay QR.",
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

            ProviderReservationResult providerReservation;

            try
            {
                providerReservation = await _providerReservationService.ReserveOrderAsync(
                    orderId,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                await WritePaymentAuditSafeAsync(
                    action: "Provider Reservation Failed",
                    status: "Failed",
                    entityId: null,
                    description: "Create provider reservation failed before creating SePay payment QR.",
                    newValues: new
                    {
                        OrderId = orderId,
                        order.OrderCode,
                        order.CustomerId,
                        Provider = "PEACOM",
                        Error = ex.Message,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Result<PaymentQrResponseDto>.Failure(
                    "Không thể giữ hàng từ nhà cung cấp. Vui lòng thử lại sau.");
            }

            var requestId = GeneratePaymentRequestId();

            var payment = new PaymentTransaction(
                orderId: order.OrderId,
                orderCode: order.OrderCode,
                customerId: order.CustomerId,
                amount: decimal.Round(order.TotalAmount, 0),
                currency: order.Currency,

                paymentProviderId: new Guid(""),//TODO
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
                        await WritePaymentAuditSafeAsync(
                            action: "Payment QR Reused After Concurrent Request",
                            status: "Success",
                            entityId: concurrentPending.Id,
                            description: "Reuse existing SePay QR after concurrent create request.",
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
                                concurrentPending.BankCode,
                                concurrentPending.BankAccountNo,
                                concurrentPending.BankAccountName,
                                concurrentPending.TransferContent,
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

            await WritePaymentAuditSafeAsync(
                action: "Payment QR Create Requested",
                status: "Success",
                entityId: payment.Id,
                description: "SePay payment QR create request initialized.",
                newValues: new
                {
                    payment.Id,
                    payment.OrderId,
                    payment.OrderCode,
                    payment.Amount,
                    payment.Currency,
                    payment.RequestId,
                    //Provider = payment.Provider.ToString(),
                    Method = payment.Method.ToString(),
                    IpAddress = ipAddress
                },
                cancellationToken: cancellationToken);

            try
            {
                var transferContent = BuildSepayTransferContent(order.OrderCode);

                var qrImageUrl = BuildSepayQrImageUrl(
                    amount: payment.Amount,
                    transferContent: transferContent);

                var expiredAt = payment.ExpiredAt;
                var providerSafeExpiredAt = providerReservation.ReservedUntil.AddSeconds(-30);

                if (!expiredAt.HasValue || expiredAt.Value <= DateTime.UtcNow)
                {
                    expiredAt = providerSafeExpiredAt;
                }
                else if (expiredAt.Value > providerSafeExpiredAt)
                {
                    expiredAt = providerSafeExpiredAt;
                }

                var rawProviderData = JsonSerializer.Serialize(new
                {
                    Provider = "SEPAY",
                    //QrBaseUrl = _sepayOptions.QrBaseUrl,
                    BankCode = _sepayOptions.BankCode,
                    AccountNumber = _sepayOptions.AccountNumber,
                    AccountName = _sepayOptions.AccountName,
                    Amount = payment.Amount,
                    Currency = payment.Currency,
                    TransferContent = transferContent,
                    QrImageUrl = qrImageUrl,
                    ExpiredAt = expiredAt,
                    RequestId = payment.RequestId
                });

                payment.MarkQrCreated(
                    providerTransactionId: payment.RequestId,
                    providerPaymentCode: transferContent,
                    qrCode: qrImageUrl,
                    qrImageUrl: qrImageUrl,
                    paymentUrl: qrImageUrl,
                    expiredAt: expiredAt,
                    bankCode: _sepayOptions.BankCode,
                    bankAccountNo: _sepayOptions.AccountNumber,
                    bankAccountName: _sepayOptions.AccountName,
                    transferContent: transferContent,
                    providerResponseCode: "200",
                    providerResponseMessage: "SePay QR created successfully.",
                    rawProviderRequest: rawProviderData,
                    rawProviderResponse: rawProviderData);

                _paymentRepository.Update(payment);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await WritePaymentAuditSafeAsync(
                    action: "Payment QR Created",
                    status: "Success",
                    entityId: payment.Id,
                    description: "SePay QR created successfully.",
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
            catch (Exception ex)
            {
                payment.MarkCreateQrFailed(
                    providerResponseCode: "EXCEPTION",
                    providerResponseMessage: ex.Message,
                    rawProviderResponse: ex.ToString());

                _paymentRepository.Update(payment);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await WritePaymentAuditSafeAsync(
                    action: "Payment QR Create Exception",
                    status: "Failed",
                    entityId: payment.Id,
                    description: "Exception occurred while creating SePay QR.",
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
                    "Không thể tạo mã QR SePay.");
            }
        }

        public async Task<Result<bool>> HandleSepayWebhookAsync(
            SepayWebhookDto callback,
            string rawBody,
            string? signature,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            ipAddress = string.IsNullOrWhiteSpace(ipAddress)
                ? "unknown"
                : ipAddress.Trim();

            rawBody ??= string.Empty;

            PaymentCallbackLog? callbackLog = null;

            try
            {
                if (callback == null)
                {
                    await WritePaymentAuditSafeAsync(
                        action: "SePay Webhook Invalid",
                        status: "Failed",
                        entityId: null,
                        description: "SePay webhook body is null.",
                        newValues: new
                        {
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    // Payload lỗi thật thì cho controller trả lỗi.
                    return Result<bool>.Failure("Webhook không hợp lệ.");
                }

                var paymentProvider = await _paymentProviderRepository.GetActiveByCodeAsync( "SEPAY", cancellationToken);
                if (paymentProvider == null)
                {
                     await WritePaymentAuditSafeAsync(
                        action: "SePay Webhook Invalid",
                        status: "Failed",
                        entityId: null,
                        description: "SePay webhook received but payment provider SEPAY is not configured.",
                        newValues: new
                        {
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Result<bool>.Failure("Payment provider SEPAY is not configured.");
                }

                var sepayTransactionId = callback.Id.ToString();
                var gateway = callback.Gateway?.Trim();
                var transactionDate = callback.TransactionDate?.Trim();
                var accountNumber = callback.AccountNumber?.Trim();
                var transferType = callback.TransferType?.Trim();
                var content = callback.Content?.Trim() ?? string.Empty;
                var description = callback.Description?.Trim() ?? string.Empty;
                var referenceCode = callback.ReferenceCode?.Trim();
                var code = callback.Code?.Trim();
                var vaNumber = callback.SubAccount?.Trim();

                callbackLog = new PaymentCallbackLog(
                    paymentProvider.Id,
                    requestId: sepayTransactionId,
                    providerTransactionId: sepayTransactionId,
                    rawBody: rawBody,
                    signature: signature,
                    ipAddress: ipAddress,
                    status: PaymentCallbackStatus.Received);

                await _callbackLogRepository.AddAsync(callbackLog, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 1. Validate required fields theo payload SePay.
                if (callback.Id <= 0 ||
                    string.IsNullOrWhiteSpace(gateway) ||
                    string.IsNullOrWhiteSpace(transactionDate) ||
                    string.IsNullOrWhiteSpace(accountNumber) ||
                    string.IsNullOrWhiteSpace(transferType) ||
                    string.IsNullOrWhiteSpace(content) ||
                    callback.TransferAmount <= 0)
                {
                    callbackLog.MarkFailed("Thiếu hoặc sai trường bắt buộc.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "SePay Webhook Missing Required Fields",
                        status: "Failed",
                        entityId: callbackLog.Id,
                        description: "SePay webhook missing required fields.",
                        newValues: new
                        {
                            callback.Id,
                            callback.Gateway,
                            callback.TransactionDate,
                            callback.AccountNumber,
                            callback.TransferType,
                            callback.TransferAmount,
                            callback.Content,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    // Đây là lỗi nghiệp vụ/payload, đã nhận request rồi thì trả success để tránh retry vô ích.
                    return Result<bool>.Success(true);
                }

                callbackLog.MarkVerified();

                // 2. Chống trùng bằng id giao dịch SePay.
                var isDuplicate = await _callbackLogRepository.ExistsProcessedByProviderTransactionIdAsync(
                    paymentProvider.Id,
                    sepayTransactionId,
                    cancellationToken);

                if (isDuplicate)
                {
                    callbackLog.MarkDuplicated("Trùng SePay transaction id.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "SePay Webhook Duplicated",
                        status: "Success",
                        entityId: callbackLog.Id,
                        description: "Duplicated SePay webhook ignored.",
                        newValues: new
                        {
                            callback.Id,
                            callback.ReferenceCode,
                            callback.Code,
                            callback.TransferAmount,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Result<bool>.Success(true);
                }

                // 3. Chỉ xử lý tiền vào.
                if (!string.Equals(transferType, "in", StringComparison.OrdinalIgnoreCase))
                {
                    callbackLog.MarkFailed("Không phải giao dịch tiền vào.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "SePay Webhook Ignored Out Transfer",
                        status: "Success",
                        entityId: callbackLog.Id,
                        description: "SePay webhook ignored because transferType is not in.",
                        newValues: new
                        {
                            callback.Id,
                            callback.TransferType,
                            callback.TransferAmount,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Result<bool>.Success(true);
                }

                // 4. Check đúng tài khoản nhận tiền.
                if (!string.Equals(
                        vaNumber,
                        _sepayOptions.AccountNumber,
                        StringComparison.OrdinalIgnoreCase))
                {
                    callbackLog.MarkFailed("Sai số tài khoản nhận tiền.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "SePay Webhook Invalid Account",
                        status: "Failed",
                        entityId: callbackLog.Id,
                        description: "SePay webhook account number does not match configured account.",
                        newValues: new
                        {
                            callback.Id,
                            CallbackAccountNumber = accountNumber,
                            ExpectedAccountNumber = _sepayOptions.AccountNumber,
                            callback.TransferAmount,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Result<bool>.Success(true);
                }

                // 5. Lấy mã thanh toán.
                // Ưu tiên callback.Code vì SePay đã bóc theo cấu hình mã thanh toán.
                // Nếu Code null thì fallback bóc từ content/description.
                var transferContent = ExtractSepayPaymentCode(callback);

                if (string.IsNullOrWhiteSpace(transferContent))
                {
                    callbackLog.MarkFailed("Không tìm thấy mã thanh toán.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "SePay Webhook Payment Code Not Found",
                        status: "Failed",
                        entityId: callbackLog.Id,
                        description: "Cannot extract payment code from SePay webhook.",
                        newValues: new
                        {
                            callback.Id,
                            callback.Code,
                            callback.Content,
                            callback.Description,
                            callback.TransferAmount,
                            Prefix = _sepayOptions.PaymentCodePrefix,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Result<bool>.Success(true);
                }

                // 6. Tìm PaymentTransaction đang Pending theo TransferContent.
                var payment = await _paymentRepository.GetPendingByTransferContentAsync(
                    paymentProvider.Id,
                    transferContent,
                    cancellationToken);

                if (payment == null)
                {
                    callbackLog.MarkFailed("Không tìm thấy giao dịch thanh toán phù hợp.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "SePay Webhook Payment Not Found",
                        status: "Failed",
                        entityId: callbackLog.Id,
                        description: "Cannot find pending SePay payment by transfer content.",
                        newValues: new
                        {
                            callback.Id,
                            TransferContent = transferContent,
                            callback.TransferAmount,
                            callback.AccountNumber,
                            callback.ReferenceCode,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Result<bool>.Success(true);
                }

                callbackLog.AttachPayment(payment.Id);

                // 7. Check số tiền.
                var expectedAmount = decimal.Round(payment.Amount, 0);
                var actualAmount = decimal.Round(callback.TransferAmount, 0);

                if (expectedAmount != actualAmount)
                {
                    callbackLog.MarkFailed("Sai số tiền thanh toán.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "SePay Webhook Amount Mismatch",
                        status: "Failed",
                        entityId: payment.Id,
                        description: "SePay webhook amount does not match payment amount.",
                        newValues: new
                        {
                            payment.Id,
                            payment.OrderId,
                            payment.OrderCode,
                            ExpectedAmount = expectedAmount,
                            ActualAmount = actualAmount,
                            callbackId = callback.Id,
                            TransferContent = transferContent,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Result<bool>.Success(true);
                }

                // 8. Check payment hết hạn.
                // SePay QR thường không tự hết hạn, nên mình phải tự bảo vệ theo ExpiredAt.
                if (payment.ExpiredAt.HasValue &&
                    payment.ExpiredAt.Value <= DateTime.UtcNow)
                {
                    payment.MarkExpired();
                    _paymentRepository.Update(payment);

                    callbackLog.MarkFailed("Thanh toán sau thời hạn cho phép.");
                    _callbackLogRepository.Update(callbackLog);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "SePay Webhook Late Payment",
                        status: "Failed",
                        entityId: payment.Id,
                        description: "Payment arrived after payment transaction expired. Manual review or refund is required.",
                        newValues: new
                        {
                            payment.Id,
                            payment.OrderId,
                            payment.OrderCode,
                            payment.Amount,
                            payment.ExpiredAt,
                            callbackId = callback.Id,
                            callback.TransferAmount,
                            TransferContent = transferContent,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Result<bool>.Success(true);
                }

                // 9. Mark payment paid.
                if (!payment.IsPaid())
                {
                    payment.MarkPaid(
                        providerTransactionId: sepayTransactionId,
                        providerResponseCode: "200",
                        providerResponseMessage: "SePay webhook payment success.",
                        rawBody);

                    _paymentRepository.Update(payment);
                }

                // 10. Mark order paid.
                var markOrderPaidResult = await _orderPaymentService.MarkOrderPaidAsync(
                    payment.OrderId,
                    payment.Id,
                    sepayTransactionId,
                    payment.PaidAt ?? DateTime.UtcNow,
                    CancellationToken.None);

                if (!markOrderPaidResult.IsSuccess)
                {
                    callbackLog.MarkFailed("Xử lý đơn hàng thất bại.");
                    _callbackLogRepository.Update(callbackLog);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "SePay Webhook Mark Order Paid Failed",
                        status: "Failed",
                        entityId: payment.Id,
                        description: "Payment was paid but marking order paid failed.",
                        newValues: new
                        {
                            payment.Id,
                            payment.OrderId,
                            payment.OrderCode,
                            callbackId = callback.Id,
                            callback.ReferenceCode,
                            callback.TransferAmount,
                            Error = markOrderPaidResult.Error,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    // Đây là lỗi hệ thống/nghiệp vụ quan trọng, cho SePay retry.
                    return Result<bool>.Failure("Xử lý đơn hàng thất bại.");
                }

                callbackLog.MarkProcessed();

                _callbackLogRepository.Update(callbackLog);
                _paymentRepository.Update(payment);

                await _unitOfWork.SaveChangesAsync(CancellationToken.None);


                await _paymentRealtimeNotifier.NotifyPaymentPaidAsync(
                    payment.OrderId,
                    payment.Id,
                    payment.OrderCode,
                    cancellationToken);


                // 11. Sau khi paid, gọi Provider confirm + redeem giống VNPT ePay.
                // Fulfillment lỗi thì không trả lỗi cho SePay, vì tiền đã nhận.
                try
                {
                    await _providerFulfillmentService.ConfirmAndRedeemAsync(
                        payment.OrderId,
                        CancellationToken.None);

                    await WritePaymentAuditSafeAsync(
                        action: "Provider Fulfillment Started",
                        status: "Success",
                        entityId: payment.OrderId,
                        description: "Provider fulfillment confirm and redeem started successfully from SePay payment.",
                        newValues: new
                        {
                            OrderId = payment.OrderId,
                            payment.OrderCode,
                            PaymentId = payment.Id,
                            Provider = "PEACOM",
                            PaymentProvider = "SEPAY"
                        },
                        cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    await _orderPaymentService.MarkFulfillmentFailedAsync(
                        payment.OrderId,
                        ex.Message,
                        cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "Provider Fulfillment Failed",
                        status: "Failed",
                        entityId: payment.OrderId,
                        description: "SePay payment was successful but provider fulfillment failed.",
                        newValues: new
                        {
                            OrderId = payment.OrderId,
                            payment.OrderCode,
                            PaymentId = payment.Id,
                            Provider = "PEACOM",
                            PaymentProvider = "SEPAY",
                            Error = ex.Message
                        },
                        cancellationToken: cancellationToken);
                }

                await WritePaymentAuditSafeAsync(
                    action: "SePay Webhook Success",
                    status: "Success",
                    entityId: payment.Id,
                    description: "Payment marked as paid from SePay webhook.",
                    newValues: new
                    {
                        payment.Id,
                        payment.OrderId,
                        payment.OrderCode,
                        payment.Amount,
                        payment.Currency,
                        payment.RequestId,
                        ProviderTransactionId = sepayTransactionId,
                        payment.PaidAt,
                        callback.ReferenceCode,
                        callback.AccountNumber,
                        callback.TransferAmount,
                        callback.Code,
                        callback.Content,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                if (callbackLog != null)
                {
                    callbackLog.MarkFailed(ex.Message);
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                await WritePaymentAuditSafeAsync(
                    action: "SePay Webhook System Error",
                    status: "Failed",
                    entityId: callbackLog?.Id,
                    description: "System error while handling SePay webhook.",
                    newValues: new
                    {
                        Error = ex.Message,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                // Lỗi hệ thống thì trả failure để controller trả 500, SePay retry.
                return Result<bool>.Failure("Lỗi hệ thống khi xử lý SePay webhook.");
            }
        }

        #endregion


        #region  helper methods

        private string? ExtractSepayPaymentCode(SepayWebhookDto callback)
        {
            if (!string.IsNullOrWhiteSpace(callback.Code))
                return NormalizePaymentCode(callback.Code);

            var prefix = string.IsNullOrWhiteSpace(_sepayOptions.PaymentCodePrefix)
                ? "DTP"
                : _sepayOptions.PaymentCodePrefix.Trim();

            var pattern = $@"\b{Regex.Escape(prefix)}[A-Z0-9]+\b";

            var rawText = $"{callback.Content} {callback.Description}".ToUpperInvariant();

            var match = Regex.Match(
                rawText,
                pattern,
                RegexOptions.IgnoreCase);

            if (!match.Success)
                return null;

            return NormalizePaymentCode(match.Value);
        }

        private static string NormalizePaymentCode(string value)
        {
            return new string(
                value
                    .Trim()
                    .Where(char.IsLetterOrDigit)
                    .ToArray())
                .ToUpperInvariant();
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
        private string? ValidateSepayOptions()
        {
            //if (string.IsNullOrWhiteSpace(_sepayOptions.QrBaseUrl))
            //    return "SePay QrBaseUrl is missing.";

            if (string.IsNullOrWhiteSpace(_sepayOptions.BankCode))
                return "SePay BankCode is missing.";

            if (string.IsNullOrWhiteSpace(_sepayOptions.AccountNumber))
                return "SePay AccountNumber is missing.";

            if (string.IsNullOrWhiteSpace(_sepayOptions.AccountName))
                return "SePay AccountName is missing.";

            //if (_sepayOptions.ExpireMinutes <= 0)
            //    return "SePay ExpireMinutes is invalid.";

            return null;
        }

        private string BuildSepayTransferContent(string orderCode)
        {
            var prefix = string.IsNullOrWhiteSpace(_sepayOptions.PaymentCodePrefix)
                ? "DTP"
                : _sepayOptions.PaymentCodePrefix.Trim().ToUpperInvariant();

            var cleanOrderCode = new string(
                (orderCode ?? string.Empty)
                    .Where(char.IsLetterOrDigit)
                    .ToArray())
                .ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(cleanOrderCode))
                cleanOrderCode = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            if (cleanOrderCode.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return cleanOrderCode;

            return $"{prefix}{cleanOrderCode}";
        }

        private static bool HasQrData(PaymentTransaction payment)
        {
            return !string.IsNullOrWhiteSpace(payment.QrCode) ||
                   !string.IsNullOrWhiteSpace(payment.QrImageUrl) ||
                   !string.IsNullOrWhiteSpace(payment.PaymentUrl);
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

        private static string GeneratePaymentRequestId()
        {
            return $"DTP-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..32];
        }

        private string BuildSepayQrImageUrl(decimal amount, string transferContent)
        {
            var amountInt = decimal.ToInt64(decimal.Round(amount, 0));

            var query = new Dictionary<string, string?>
            {
                ["acc"] = _sepayOptions.AccountNumber.Trim(),
                ["bank"] = _sepayOptions.BankCode.Trim(),
                ["amount"] = amountInt.ToString(CultureInfo.InvariantCulture),
                ["des"] = transferContent
            };

            if (!string.IsNullOrWhiteSpace(_sepayOptions.Template))
            {
                query["template"] = _sepayOptions.Template.Trim();
            }

            return QueryHelpers.AddQueryString(
                _sepayOptions.QrBaseUrl.Trim(),
                query);
        }

        #endregion
    }
}
