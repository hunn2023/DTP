using DTP.Modules.Delivery.Application.Commands.DeliverOrder;
using DTP.Modules.Delivery.Application.Commands.Delivery;
using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Payment.Domain.Entities;
using DTP.Modules.Payment.Domain.Enums;
using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.DTOs;
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


        public async Task<Result<PaymentQrResponseDto>> CreateQrAsync(
             Guid orderId,
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

                return Result<PaymentQrResponseDto>.Failure("Số tiền thanh toán không hợp lệ.");
            }

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

                return Result<PaymentQrResponseDto>.Failure("Đơn hàng đã được thanh toán.");
            }

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
                            description: "Reuse existing pending payment QR.",
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
                    description: "Existing payment QR expired before creating new QR.",
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
                    description: "Create provider reservation failed before creating payment QR.",
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
                        await WritePaymentAuditSafeAsync(
                            action: "Payment QR Reused After Concurrent Request",
                            status: "Success",
                            entityId: concurrentPending.Id,
                            description: "Reuse existing QR after concurrent create request.",
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

            await WritePaymentAuditSafeAsync(
                action: "Payment QR Create Requested",
                status: "Success",
                entityId: payment.Id,
                description: "Payment QR create request initialized.",
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



            var epay = new VnptEpayRegisterVaRequest
            {
                Amount = (long)order.TotalAmount,
                CustomerName = order.CustomerName ?? "KHACH HANG",
                Phone = order.CustomerPhone,
                Email = order.CustomerEmail,
                Address = "",
                ReferenceId = payment.OrderId.ToString(),
                ContentQr = order.OrderCode
            };
            VnptEpayRegisterVaResponse epayResponse;

            try
            {
                epayResponse = await _vnptEpayClient.RegisterVaAsync(epay, cancellationToken);
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
                    description: "Exception occurred while creating VNPT ePay QR.",
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

            if (!epayResponse.IsSuccess)
            {
                payment.MarkCreateQrFailed(
                    providerResponseCode: epayResponse.ResponseCode,
                    providerResponseMessage: epayResponse.Message,
                    rawProviderResponse: epayResponse.Message);

                _paymentRepository.Update(payment);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await WritePaymentAuditSafeAsync(
                    action: "Payment QR Create Failed",
                    status: "Failed",
                    entityId: payment.Id,
                    description: "VNPT ePay failed to create QR.",
                    newValues: new
                    {
                        payment.Id,
                        payment.OrderId,
                        payment.OrderCode,
                        payment.Amount,
                        payment.Currency,
                        payment.RequestId,
                        epayResponse.ResponseCode,
                        epayResponse.Message,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Result<PaymentQrResponseDto>.Failure(
                    epayResponse.Message ?? "Không tạo được mã QR thanh toán.");
            }

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


            payment.MarkQrCreated(
                providerTransactionId: epayResponse.MapId,
                providerPaymentCode: epayResponse.ResponseCode,
                qrCode: epayResponse.QrCode,
                qrImageUrl: epayResponse.QrUrl,
                paymentUrl: epayResponse.QrUrl,
                expiredAt: expiredAt,
                bankCode: epayResponse.BankCode,
                bankAccountNo: epayResponse.AccountNo,
                bankAccountName: epayResponse.AccountName,
                transferContent: epayResponse.MapId,
                providerResponseCode: epayResponse.ResponseCode,
                providerResponseMessage: epayResponse.Message,
                rawProviderRequest: epayResponse.QrDataRaw,
                rawProviderResponse: epayResponse.QrDataRaw);

            _paymentRepository.Update(payment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await WritePaymentAuditSafeAsync(
                action: "Payment QR Created",
                status: "Success",
                entityId: payment.Id,
                description: "VNPT ePay QR created successfully.",
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

        public async Task<EPayResponse> HandleVnptEpayCallbackAsync(
           Application.DTOs.VnptEpayCallbackDto callback,
           string rawBody,
           string? ipAddress,
           CancellationToken cancellationToken = default)
        {
            const string MerchantCode = "THE365";

            EPayResponse Response(string code, string message)
                => new EPayResponse
                {
                    ResponseCode = code,
                    ResponseMessage = message
                };

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
                        action: "VNPT ePay Callback Invalid",
                        status: "Failed",
                        entityId: null,
                        description: "Callback body is null.",
                        newValues: new
                        {
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Response("124", "Callback không hợp lệ");
                }

                var requestId = callback.RequestId?.Trim();
                var requestTime = callback.RequestTime?.Trim();
                var referenceId = callback.ReferenceId?.Trim();
                var mapId = callback.MapId?.Trim();
                var signature = callback.Signature?.Trim();
                var merchantCode = callback.MerchantCode?.Trim();
                var vaAcc = callback.VaAcc?.Trim();
                var vaName = callback.VaName?.Trim();

                callbackLog = new PaymentCallbackLog(
                    provider: PaymentProvider.VnptEpay,
                    requestId: requestId ?? string.Empty,
                    providerTransactionId: referenceId,
                    rawBody: rawBody,
                    signature: signature,
                    ipAddress: ipAddress,
                    status: PaymentCallbackStatus.Received);

                await _callbackLogRepository.AddAsync(callbackLog, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 1. Validate 
                if (string.IsNullOrWhiteSpace(requestId) ||
                    string.IsNullOrWhiteSpace(requestTime) ||
                    string.IsNullOrWhiteSpace(referenceId) ||
                    string.IsNullOrWhiteSpace(mapId) ||
                    string.IsNullOrWhiteSpace(signature) ||
                    string.IsNullOrWhiteSpace(merchantCode) ||
                    string.IsNullOrWhiteSpace(vaAcc) ||
                    string.IsNullOrWhiteSpace(vaName))
                {
                    callbackLog.MarkFailed("Thiếu trường bắt buộc.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "VNPT ePay Callback Missing Required Fields",
                        status: "Failed",
                        entityId: callbackLog.Id,
                        description: "VNPT ePay callback missing required fields.",
                        newValues: new
                        {
                            callback.RequestId,
                            callback.RequestTime,
                            callback.ReferenceId,
                            callback.MapId,
                            callback.MerchantCode,
                            callback.VaAcc,
                            callback.VaName,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Response("124", "Thiếu trường bắt buộc");
                }

                // 2. Check MerchantCode
                if (!string.Equals(merchantCode, MerchantCode, StringComparison.Ordinal))
                {
                    callbackLog.MarkFailed("Sai MerchantCode.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "VNPT ePay Callback Invalid MerchantCode",
                        status: "Failed",
                        entityId: callbackLog.Id,
                        description: "VNPT ePay callback merchant code is invalid.",
                        newValues: new
                        {
                            callback.RequestId,
                            callback.ReferenceId,
                            callback.MapId,
                            CallbackMerchantCode = callback.MerchantCode,
                            ExpectedMerchantCode = MerchantCode,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Response("110", "Sai MerchantCode");
                }

                // 3. Check Amount
                if (callback.Amount <= 0)
                {
                    callbackLog.MarkFailed("Sai số tiền.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "VNPT ePay Callback Invalid Amount",
                        status: "Failed",
                        entityId: callbackLog.Id,
                        description: "VNPT ePay callback amount is invalid.",
                        newValues: new
                        {
                            callback.RequestId,
                            callback.ReferenceId,
                            callback.MapId,
                            callback.Amount,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Response("125", "Sai số tiền");
                }

                // 4. Verify chữ ký
                var isValidSignature = _vnptEpayClient.VerifyDepositNotificationSignature(callback);

                if (!isValidSignature)
                {
                    callbackLog.MarkInvalidSignature("Sai chữ ký.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "VNPT ePay Callback Invalid Signature",
                        status: "Failed",
                        entityId: callbackLog.Id,
                        description: "VNPT ePay callback signature validation failed.",
                        newValues: new
                        {
                            callback.RequestId,
                            callback.ReferenceId,
                            callback.RequestTime,
                            callback.Amount,
                            callback.Fee,
                            callback.VaAcc,
                            callback.MapId,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Response("103", "Sai chữ ký");
                }

                callbackLog.MarkVerified();

                // 5. Chống trùng ReferenceId
                // ReferenceId tương đương mã giao dịch nạp tiền của VNPT.
                var isDuplicate = await _callbackLogRepository.ExistsProcessedByProviderTransactionIdAsync(
                    PaymentProvider.VnptEpay,
                    referenceId,
                    cancellationToken);

                if (isDuplicate)
                {
                    callbackLog.MarkDuplicated("Trùng ReferenceId.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "VNPT ePay Callback Duplicated",
                        status: "Success",
                        entityId: callbackLog.Id,
                        description: "Duplicated VNPT ePay ReferenceId ignored.",
                        newValues: new
                        {
                            callback.RequestId,
                            callback.ReferenceId,
                            callback.MapId,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Response("102", "Trùng ReferenceId");
                }

                // 6. Tìm payment/order theo MapId + Amount + trạng thái chờ thanh toán
                var payment = await _paymentRepository.GetPendingVnptEpayByMapIdAndAmountAsync(
                    mapId,
                    callback.Amount,
                    cancellationToken);

                if (payment == null)
                {
                    callbackLog.MarkFailed("Không tìm thấy đơn hàng phù hợp.");
                    _callbackLogRepository.Update(callbackLog);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "VNPT ePay Callback Payment Not Found",
                        status: "Failed",
                        entityId: callbackLog.Id,
                        description: "Cannot find pending payment by MapId and Amount.",
                        newValues: new
                        {
                            callback.RequestId,
                            callback.ReferenceId,
                            callback.MapId,
                            callback.Amount,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Response("101", "Không tìm thấy đơn hàng phù hợp");
                }

                callbackLog.AttachPayment(payment.Id);

                // 7. Nếu payment đã Paid nhưng callback chưa processed,
                // vẫn gọi MarkOrderPaidAsync lại để tự hồi phục nếu lần trước lỗi giữa chừng.
                if (!payment.IsPaid())
                {
                    payment.MarkPaid(
                        providerTransactionId: referenceId,
                         "200",
                        "Nạp tiền thành công",
                         rawBody);

                    _paymentRepository.Update(payment);
                }

                var markOrderPaidResult = await _orderPaymentService.MarkOrderPaidAsync(
                    payment.OrderId,
                    payment.Id,
                    referenceId,
                    payment.PaidAt ?? DateTime.UtcNow,
                    cancellationToken);

                if (!markOrderPaidResult.IsSuccess)
                {
                    callbackLog.MarkFailed("Xử lý đơn hàng thất bại.");
                    _callbackLogRepository.Update(callbackLog);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "VNPT ePay Callback Mark Order Paid Failed",
                        status: "Failed",
                        entityId: payment.Id,
                        description: "Payment was paid but marking order paid failed.",
                        newValues: new
                        {
                            payment.Id,
                            payment.OrderId,
                            payment.OrderCode,
                            callback.ReferenceId,
                            callback.MapId,
                            callback.Amount,
                            Error = markOrderPaidResult.Error,
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Response("199", "Xử lý đơn hàng thất bại");
                }

                callbackLog.MarkProcessed();

                _callbackLogRepository.Update(callbackLog);
                _paymentRepository.Update(payment);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 8. Sau khi đơn đã paid, gọi fulfillment provider.
                // Lưu ý: Fulfillment lỗi thì KHÔNG nên trả lỗi cho VNPT,
                // vì tiền đã nhận thành công. Chỉ mark fulfillment failed để worker/admin xử lý lại.
                try
                {
                    await _providerFulfillmentService.ConfirmAndRedeemAsync(
                        payment.OrderId,
                        cancellationToken);

                    await WritePaymentAuditSafeAsync(
                        action: "Provider Fulfillment Started",
                        status: "Success",
                        entityId: payment.OrderId,
                        description: "Provider fulfillment confirm and redeem started successfully.",
                        newValues: new
                        {
                            OrderId = payment.OrderId,
                            payment.OrderCode,
                            PaymentId = payment.Id,
                            Provider = "PEACOM"
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
                        description: "Payment was successful but provider fulfillment failed.",
                        newValues: new
                        {
                            OrderId = payment.OrderId,
                            payment.OrderCode,
                            PaymentId = payment.Id,
                            Provider = "PEACOM",
                            Error = ex.Message
                        },
                        cancellationToken: cancellationToken);
                }

                await WritePaymentAuditSafeAsync(
                    action: "VNPT ePay Callback Success",
                    status: "Success",
                    entityId: payment.Id,
                    description: "Payment marked as paid from VNPT ePay deposit notification.",
                    newValues: new
                    {
                        payment.Id,
                        payment.OrderId,
                        payment.OrderCode,
                        payment.Amount,
                        payment.Currency,
                        payment.RequestId,
                        ProviderTransactionId = referenceId,
                        payment.PaidAt,
                        callback.MapId,
                        callback.VaAcc,
                        callback.VaName,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Response("200", "Đẩy thông báo thành công");
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
                    action: "VNPT ePay Callback System Error",
                    status: "Failed",
                    entityId: callbackLog?.Id,
                    description: "System error while handling VNPT ePay callback.",
                    newValues: new
                    {
                        Error = ex.Message,
                        IpAddress = ipAddress
                    },
                    cancellationToken: cancellationToken);

                return Response("500", "Lỗi hệ thống");
            }
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

        private static string GeneratePaymentRequestId()
        {
            return $"DTP-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..32];
        }
    }
}
