using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Domain.Enums;
using DTP.Modules.Delivery.Application.Abstractions.Repositories;
using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Application.DTOs;
using DTP.Modules.Delivery.Domain.Entities;
using DTP.Modules.Delivery.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Delivery;
using DTP.Shared.Application.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DTP.Modules.Delivery.Infrastructure.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IDeliveryUnitOfWork _unitOfWork;
        private readonly IOrderDeliveryReader _orderDeliveryReader;
        private readonly IAuditLogWriter _auditLogService;
        private readonly IEsimDeliveryEmailService _esimDeliveryEmailService;
        private readonly IDeliveryRateLimitService _deliveryRateLimitService;

        public DeliveryService(
            IDeliveryRepository deliveryRepository,
            IDeliveryUnitOfWork unitOfWork,
            IOrderDeliveryReader orderDeliveryReader,
            IAuditLogWriter auditLogService,
            IEsimDeliveryEmailService esimDeliveryEmailService,
            IDeliveryRateLimitService deliveryRateLimitService)
        {
            _deliveryRepository = deliveryRepository;
            _unitOfWork = unitOfWork;
            _orderDeliveryReader = orderDeliveryReader;
            _auditLogService = auditLogService;
            _esimDeliveryEmailService = esimDeliveryEmailService;
            _deliveryRateLimitService = deliveryRateLimitService;
        }

        public async Task<Result<Guid>> CreatePendingAsync(
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            if (orderId == Guid.Empty)
                return Result<Guid>.Failure("OrderId không hợp lệ.");

            ipAddress = string.IsNullOrWhiteSpace(ipAddress)
                   ? "unknown"
                   : ipAddress.Trim();

            var exists = await _deliveryRepository.ExistsByOrderIdAsync(
                orderId,
                cancellationToken);

            if (exists)
            {

                var existing = await _deliveryRepository.GetByOrderIdAsync(
                    orderId,
                    cancellationToken);


                if (existing != null)
                {
                    if (existing.Status == DeliveryStatus.Delivered)
                        return Result<Guid>.Success(existing.Id);

                    if (existing.Status == DeliveryStatus.Processing)
                        return Result<Guid>.Failure("Đơn giao hàng đang được xử lý.");

                    return Result<Guid>.Success(existing.Id);
                }

            }

            var order = await _orderDeliveryReader.GetOrderForDeliveryAsync(
                orderId,
                cancellationToken);

            if (order == null)
                return Result<Guid>.Failure("Không tìm thấy đơn hàng.");

            if (!order.IsPaid)
                return Result<Guid>.Failure("Đơn hàng chưa thanh toán, không thể tạo lệnh giao hàng.");

            if (order.Items.Count == 0)
                return Result<Guid>.Failure("Đơn hàng không có sản phẩm để giao.");

            var delivery = new Domain.Entities.Delivery(
                order.OrderId,
                order.OrderCode,
                order.CustomerId,
                order.CustomerName,
                order.CustomerEmail,
                  MapDeliveryType(order.DeliveryType),
                ipAddress);


            foreach (var item in order.Items)
            {
                var quantity = item.Quantity <= 0 ? 1 : item.Quantity;

                for (var i = 0; i < quantity; i++)
                {
                    delivery.AddItem(
                        item.OrderItemId,
                        item.ProductId,
                        item.ProductVariantId,
                        item.ProductName,
                        item.Sku,
                        1);
                }
            }


            await _deliveryRepository.AddAsync(delivery, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await WriteAuditSafeAsync(
                action: "Create Delivery",
                actionType: AuditActionType.Create,
                status: AuditStatus.Success,
                entityName: "Delivery",
                entityId: delivery.Id,
                description: "Delivery created after order payment.",
                newValues: new
                {
                    delivery.Id,
                    delivery.OrderId,
                    delivery.OrderCode,
                    delivery.CustomerId,
                    delivery.CustomerName,
                    delivery.CustomerEmail,
                    delivery.DeliveryType,
                    delivery.Status,
                    delivery.IpAddress,
                    Items = delivery.Items.Select(x => new
                    {
                        x.Id,
                        x.OrderItemId,
                        x.ProductId,
                        x.ProductVariantId,
                        x.ProductName,
                        x.Sku,
                        x.Quantity
                    }),
                    CreatedAt = DateTime.UtcNow
                },
                cancellationToken: cancellationToken);

            return Result<Guid>.Success(delivery.Id);
        }


        public async Task<Result> ProcessAsync(
            Guid deliveryId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            if (deliveryId == Guid.Empty)
                return Result.Failure("DeliveryId không hợp lệ.");

            ipAddress = string.IsNullOrWhiteSpace(ipAddress)
                ? "unknown"
                : ipAddress.Trim();

            var delivery = await _deliveryRepository.GetTrackingByIdAsync(
                deliveryId,
                cancellationToken);

            if (delivery == null)
                return Result.Failure("Không tìm thấy lệnh giao hàng.");

            if (delivery.Status == DeliveryStatus.Delivered)
            {
                await WriteAuditSafeAsync(
                    action: "Process Delivery Idempotent",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Success,
                    entityName: "Delivery",
                    entityId: delivery.Id,
                    description: "Delivery was already delivered. Process request ignored.",
                    newValues: new
                    {
                        delivery.Id,
                        delivery.OrderId,
                        delivery.OrderCode,
                        delivery.Status,
                        delivery.DeliveredAt,
                        IpAddress = ipAddress,
                        Reason = "Already delivered"
                    },
                    cancellationToken: cancellationToken);

                return Result.Success();
            }

            if (delivery.Status == DeliveryStatus.Processing)
            {
                await WriteAuditSafeAsync(
                    action: "Process Delivery Blocked",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Delivery",
                    entityId: delivery.Id,
                    description: "Delivery is already processing.",
                    newValues: new
                    {
                        delivery.Id,
                        delivery.OrderId,
                        delivery.OrderCode,
                        delivery.Status,
                        IpAddress = ipAddress,
                        Reason = "Already processing"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Lệnh giao hàng đang được xử lý. Vui lòng thử lại sau.");
            }

            var blocked = await _deliveryRateLimitService.IsProcessBlockedAsync(
                delivery.Id,
                delivery.OrderId,
                ipAddress,
                cancellationToken);

            if (blocked)
            {
                await WriteAuditSafeAsync(
                    action: "Process Delivery Blocked",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Delivery",
                    entityId: delivery.Id,
                    description: "Delivery processing was blocked by rate limit.",
                    newValues: new
                    {
                        delivery.Id,
                        delivery.OrderId,
                        delivery.OrderCode,
                        delivery.Status,
                        IpAddress = ipAddress,
                        Reason = "Process delivery rate limit exceeded"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Bạn xử lý giao hàng quá nhiều lần. Vui lòng thử lại sau.");
            }

            await _deliveryRateLimitService.RegisterProcessAttemptAsync(
                delivery.Id,
                delivery.OrderId,
                ipAddress,
                cancellationToken);

            var oldStatus = delivery.Status;

            try
            {
                delivery.StartProcessing();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await WriteAuditSafeAsync(
                    action: "Process Delivery",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Success,
                    entityName: "Delivery",
                    entityId: delivery.Id,
                    description: "Delivery processing started.",
                    newValues: new
                    {
                        delivery.Id,
                        delivery.OrderId,
                        delivery.OrderCode,
                        FromStatus = oldStatus.ToString(),
                        ToStatus = delivery.Status.ToString(),
                        delivery.AttemptCount,
                        IpAddress = ipAddress,
                        ProcessAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {


                foreach (var entry in ex.Entries)
                {

                    // Quan trọng: detach entity lỗi khỏi DbContext
                    entry.State = EntityState.Detached;
                }

                return Result.Failure("Lệnh giao hàng đã được tiến trình khác xử lý. Vui lòng tải lại trạng thái.");
            }
            catch (Exception ex)
            {
                await WriteAuditSafeAsync(
                    action: "Process Delivery Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Delivery",
                    entityId: delivery.Id,
                    description: "Delivery start processing failed.",
                    newValues: new
                    {
                        delivery.Id,
                        delivery.OrderId,
                        delivery.OrderCode,
                        delivery.Status,
                        IpAddress = ipAddress,
                        errorMessage = ex.Message,
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure(ex.Message);
            }

            try
            {

                var notDeliveredItems = delivery.Items
                    .Where(x => !x.IsDelivered)
                    .ToList();

                if (notDeliveredItems.Count > 0)
                {
                    await WriteAuditSafeAsync(
                        action: "Delivery Waiting Fulfillment",
                        actionType: AuditActionType.Update,
                        status: AuditStatus.Failed,
                        entityName: "Delivery",
                        entityId: delivery.Id,
                        description: "Delivery items are not fulfilled yet.",
                        newValues: new
                        {
                            delivery.Id,
                            delivery.OrderId,
                            delivery.OrderCode,
                            NotDeliveredItems = notDeliveredItems.Select(x => new
                            {
                                x.Id,
                                x.OrderItemId,
                                x.ProductName,
                                x.Sku,
                                x.IsDelivered
                            }).ToList(),
                            IpAddress = ipAddress
                        },
                        cancellationToken: cancellationToken);

                    return Result.Failure("Delivery chưa có đủ thông tin fulfillment. Vui lòng đợi provider trả kết quả.");
                }

                delivery.MarkDelivered("Digital products delivered successfully.");

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await WriteAuditSafeAsync(
                    action: "Delivery Success",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Success,
                    entityName: "Delivery",
                    entityId: delivery.Id,
                    description: "Digital delivery completed successfully.",
                    newValues: new
                    {
                        delivery.Id,
                        delivery.OrderId,
                        delivery.OrderCode,
                        delivery.Status,
                        delivery.DeliveredAt,
                        IpAddress = ipAddress,
                        Items = delivery.Items.Select(x => new
                        {
                            x.Id,
                            x.OrderItemId,
                            x.ProductName,
                            x.Sku,
                            x.QrCodeUrl,
                            x.ActivationCode,
                            x.SerialNumber,
                            x.ProviderReference,
                            x.IsDelivered,
                            x.DeliveredAt
                        }).ToList()
                    },
                    cancellationToken: cancellationToken);

                await SendDeliveryEmailSafeAsync(
                    delivery,
                    cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                delivery.MarkFailed(ex.Message);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await WriteAuditSafeAsync(
                    action: "Delivery Exception",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Delivery",
                    entityId: delivery.Id,
                    description: "Exception occurred while processing delivery.",
                    newValues: new
                    {
                        delivery.Id,
                        delivery.OrderId,
                        delivery.OrderCode,
                        delivery.Status,
                        delivery.LastError,
                        delivery.FailedAt,
                        IpAddress = ipAddress,
                        errorMessage = ex.Message,
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Có lỗi khi xử lý giao hàng.");
            }
        }


        public async Task<Result> MarkDeliveredAsync(
            Guid deliveryId,
            string? note,
            CancellationToken cancellationToken = default)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(
                deliveryId,
                cancellationToken);

            if (delivery == null)
                return Result.Failure("Không tìm thấy lệnh giao hàng.");

            delivery.MarkDelivered(note);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await WriteAuditSafeAsync(
                action: "Mark Delivery Delivered",
                actionType: AuditActionType.Update,
                status: AuditStatus.Success,
                entityName: "Delivery",
                entityId: delivery.Id,
                description: "Delivery manually marked as delivered.",
                newValues: new
                {
                    delivery.Id,
                    delivery.OrderId,
                    delivery.OrderCode,
                    delivery.Status,
                    delivery.Note,
                    delivery.DeliveredAt
                },
                cancellationToken: cancellationToken);

            return Result.Success();
        }

        public async Task<Result> MarkFailedAsync(
            Guid deliveryId,
            string error,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(error))
                return Result.Failure("Vui lòng nhập lý do thất bại.");

            var delivery = await _deliveryRepository.GetByIdAsync(
                deliveryId,
                cancellationToken);

            if (delivery == null)
                return Result.Failure("Không tìm thấy lệnh giao hàng.");

            delivery.MarkFailed(error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await WriteAuditSafeAsync(
                action: "Mark Delivery Failed",
                actionType: AuditActionType.Update,
                status: AuditStatus.Success,
                entityName: "Delivery",
                entityId: delivery.Id,
                description: "Delivery manually marked as failed.",
                newValues: new
                {
                    delivery.Id,
                    delivery.OrderId,
                    delivery.OrderCode,
                    delivery.Status,
                    delivery.LastError,
                    delivery.FailedAt
                },
                cancellationToken: cancellationToken);

            return Result.Success();
        }

        public async Task<Result<DeliveryDto>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (delivery == null)
                return Result<DeliveryDto>.Failure("Không tìm thấy lệnh giao hàng.");

            return Result<DeliveryDto>.Success(MapToDto(delivery));
        }

        public async Task<Result<DeliveryDto>> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var delivery = await _deliveryRepository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

            if (delivery == null)
                return Result<DeliveryDto>.Failure("Không tìm thấy lệnh giao hàng.");

            return Result<DeliveryDto>.Success(MapToDto(delivery));
        }


        public async Task<Result> ResendEsimEmailAsync(
            Guid deliveryId,
            CancellationToken cancellationToken = default)
            {
                if (deliveryId == Guid.Empty)
                    return Result.Failure("DeliveryId không hợp lệ.");

                var delivery = await _deliveryRepository.GetByIdAsync(
                    deliveryId,
                    cancellationToken);

                if (delivery == null)
                    return Result.Failure("Không tìm thấy lệnh giao hàng.");

                if (delivery.Status != DeliveryStatus.Delivered)
                    return Result.Failure("Chỉ có thể gửi email khi eSIM đã được giao thành công.");

                if (string.IsNullOrWhiteSpace(delivery.CustomerEmail))
                    return Result.Failure("Đơn hàng không có email khách hàng.");

                if (!delivery.Items.Any(x =>
                        !string.IsNullOrWhiteSpace(x.QrCodeUrl) ||
                        !string.IsNullOrWhiteSpace(x.ActivationCode)))
                {
                    return Result.Failure("Delivery chưa có thông tin QR Code hoặc Activation Code.");
                }

                try
                {
                    var dto = MapToDto(delivery);

                    await _esimDeliveryEmailService.SendEsimQrEmailAsync(
                        dto,
                        cancellationToken);

                    delivery.MarkEmailSent();

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WriteAuditSafeAsync(
                        action: "Resend eSIM Email Success",
                        actionType: AuditActionType.Update,
                        status: AuditStatus.Success,
                        entityName: "Delivery",
                        entityId: delivery.Id,
                        description: "eSIM QR email resent to customer successfully.",
                        newValues: new
                        {
                            delivery.Id,
                            delivery.OrderId,
                            delivery.OrderCode,
                            delivery.CustomerEmail,
                            delivery.EmailSent,
                            delivery.EmailSentAt
                        },
                        cancellationToken: cancellationToken);

                    return Result.Success();
                }
                catch (Exception ex)
                {
                    delivery.MarkEmailFailed(ex.Message);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await WriteAuditSafeAsync(
                        action: "Resend eSIM Email Failed",
                        actionType: AuditActionType.Update,
                        status: AuditStatus.Failed,
                        entityName: "Delivery",
                        entityId: delivery.Id,
                        description: "Failed to resend eSIM QR email to customer.",
                        newValues: new
                        {
                            delivery.Id,
                            delivery.OrderId,
                            delivery.OrderCode,
                            delivery.CustomerEmail,
                            Error = ex.Message
                        },
                        cancellationToken: cancellationToken);

                    return Result.Failure("Gửi lại email eSIM thất bại.");
                }
            }

        private static DeliveryDto MapToDto(Domain.Entities.Delivery delivery)
        {
            return new DeliveryDto
            {
                Id = delivery.Id,
                OrderId = delivery.OrderId,
                OrderCode = delivery.OrderCode,
                CustomerId = delivery.CustomerId,
                CustomerName = delivery.CustomerName,
                CustomerEmail = delivery.CustomerEmail,
                DeliveryType = delivery.DeliveryType,
                Status = delivery.Status,
                AttemptCount = delivery.AttemptCount,
                LastError = delivery.LastError,
                IpAddress = delivery.IpAddress,
                Note = delivery.Note,
                DeliveredAt = delivery.DeliveredAt,
                FailedAt = delivery.FailedAt,
                CreatedAt = delivery.CreatedAt,
                EmailSent = delivery.EmailSent,
                EmailSentAt = delivery.EmailSentAt,
                EmailError = delivery.EmailError,
                Items = delivery.Items.Select(x => new DeliveryItemDto
                {
                    Id = x.Id,
                    OrderItemId = x.OrderItemId,
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    ProductName = x.ProductName,
                    Sku = x.Sku,
                    Quantity = x.Quantity,
                    QrCodeUrl = x.QrCodeUrl,
                    ActivationCode = x.ActivationCode,
                    SerialNumber = x.SerialNumber,
                    ProviderReference = x.ProviderReference,
                    IsDelivered = x.IsDelivered,
                    DeliveredAt = x.DeliveredAt
                }).ToList()
            };
        }

        private static Domain.Enums.DeliveryType MapDeliveryType(
            SharedDeliveryType deliveryType)
        {
            return deliveryType switch
            {
                SharedDeliveryType.Esim => Domain.Enums.DeliveryType.Esim,
                SharedDeliveryType.PhoneCard => Domain.Enums.DeliveryType.PhoneCard,
                _ => Domain.Enums.DeliveryType.OtherDigital
            };
        }


        private async Task SendDeliveryEmailSafeAsync(
            Domain.Entities.Delivery delivery,
            CancellationToken cancellationToken)
        {
            if (delivery.EmailSent)
                return;

            var deliveryDto = MapToDto(delivery);

            try
            {
                await _esimDeliveryEmailService.SendEsimQrEmailAsync(
                    deliveryDto,
                    cancellationToken);

                delivery.MarkEmailSent();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await WriteAuditSafeAsync(
                    action: "Send eSIM Email Success",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Success,
                    entityName: "Delivery",
                    entityId: delivery.Id,
                    description: "eSIM QR email sent to customer successfully.",
                    newValues: new
                    {
                        delivery.Id,
                        delivery.OrderId,
                        delivery.OrderCode,
                        delivery.CustomerEmail,
                        delivery.EmailSent,
                        delivery.EmailSentAt
                    },
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                delivery.MarkEmailFailed(ex.Message);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await WriteAuditSafeAsync(
                    action: "Send eSIM Email Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "Delivery",
                    entityId: delivery.Id,
                    description: "Failed to send eSIM QR email to customer.",
                    newValues: new
                    {
                        delivery.Id,
                        delivery.OrderId,
                        delivery.OrderCode,
                        delivery.CustomerEmail,
                        delivery.EmailSent,
                        delivery.EmailError,
                        FailedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);
            }
        }


        public async Task<Result> ApplyProviderRedeemFulfillmentAsync(
            Guid deliveryId,
            IReadOnlyList<DeliveryFulfillmentItemDto> items,
            CancellationToken cancellationToken = default)
        {
            if (deliveryId == Guid.Empty)
                return Result.Failure("DeliveryId không hợp lệ.");

            if (items == null || items.Count == 0)
                return Result.Failure("Không có dữ liệu fulfillment để cập nhật.");

            var delivery = await _deliveryRepository.GetTrackingByIdAsync(
                deliveryId,
                cancellationToken);

            if (delivery == null)
                return Result.Failure("Không tìm thấy lệnh giao hàng.");

            if (delivery.Status == DeliveryStatus.Delivered)
                return Result.Success();

            var appliedCount = 0;
            var skippedCount = 0;
            var errors = new List<string>();

            foreach (var fulfillment in items)
            {
                if (string.IsNullOrWhiteSpace(fulfillment.SerialNumber))
                {
                    errors.Add("Thiếu serial.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(fulfillment.QrCodeUrl) &&
                    string.IsNullOrWhiteSpace(fulfillment.ActivationCode))
                {
                    errors.Add($"Serial {fulfillment.SerialNumber} thiếu QR Code hoặc Activation Code.");
                    continue;
                }

                var alreadyApplied = delivery.Items.Any(x =>
                    x.IsDelivered &&
                    string.Equals(
                        x.SerialNumber,
                        fulfillment.SerialNumber,
                        StringComparison.OrdinalIgnoreCase));

                if (alreadyApplied)
                {
                    skippedCount++;
                    continue;
                }

                var deliveryItem = FindDeliveryItemForFulfillment(
                    delivery,
                    fulfillment);

                if (deliveryItem == null)
                {
                    errors.Add(
                        $"Không tìm thấy DeliveryItem phù hợp cho serial {fulfillment.SerialNumber}, sku {fulfillment.Sku}.");

                    continue;
                }

                deliveryItem.SetFulfillment(
                    qrCodeUrl: fulfillment.QrCodeUrl,
                    activationCode: fulfillment.ActivationCode,
                    serialNumber: fulfillment.SerialNumber,
                    providerReference: fulfillment.ProviderReference,
                    rawData: fulfillment.RawData);

                deliveryItem.MarkDelivered();

                appliedCount++;
            }

            if (appliedCount == 0 && skippedCount == 0)
            {
                return Result.Failure(
                    errors.Count > 0
                        ? string.Join(" | ", errors)
                        : "Không có item nào được fulfillment.");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await WriteAuditSafeAsync(
                action: "Apply Provider Redeem Fulfillment",
                actionType: AuditActionType.Update,
                status: errors.Count == 0 ? AuditStatus.Success : AuditStatus.Failed,
                entityName: "Delivery",
                entityId: delivery.Id,
                description: "Applied provider redeem result to delivery items.",
                newValues: new
                {
                    delivery.Id,
                    delivery.OrderId,
                    delivery.OrderCode,
                    AppliedCount = appliedCount,
                    SkippedCount = skippedCount,
                    Errors = errors,
                    Items = delivery.Items.Select(x => new
                    {
                        x.Id,
                        x.OrderItemId,
                        x.ProductName,
                        x.Sku,
                        x.QrCodeUrl,
                        x.ActivationCode,
                        x.SerialNumber,
                        x.ProviderReference,
                        x.IsDelivered,
                        x.DeliveredAt
                    }).ToList()
                },
                cancellationToken: cancellationToken);

            if (errors.Count > 0)
                return Result.Failure(string.Join(" | ", errors));

            return Result.Success();
        }


        private static DeliveryItem? FindDeliveryItemForFulfillment(
    Domain.Entities.Delivery delivery,
    DeliveryFulfillmentItemDto fulfillment)
        {
            if (fulfillment.OrderItemId.HasValue)
            {
                var byOrderItemId = delivery.Items.FirstOrDefault(x =>
                    !x.IsDelivered &&
                    x.OrderItemId == fulfillment.OrderItemId.Value);

                if (byOrderItemId != null)
                    return byOrderItemId;
            }

            if (!string.IsNullOrWhiteSpace(fulfillment.Sku))
            {
                var bySku = delivery.Items.FirstOrDefault(x =>
                    !x.IsDelivered &&
                    string.Equals(
                        x.Sku,
                        fulfillment.Sku,
                        StringComparison.OrdinalIgnoreCase));

                if (bySku != null)
                    return bySku;
            }

            return delivery.Items.FirstOrDefault(x => !x.IsDelivered);
        }

        private async Task WriteAuditSafeAsync(
            string action,
            AuditActionType actionType,
            AuditStatus status,
            string entityName,
            Guid entityId,
            string description,
            object? newValues,
            CancellationToken cancellationToken)
        {
            try
            {
                await _auditLogService.WriteAsync(
                    module: "Delivery",
                    action: action,
                    actionType: actionType,
                    status: status,
                    entityName: entityName,
                    entityId: entityId,
                    description: description,
                    oldValues: null,
                    newValues: newValues,
                    cancellationToken: cancellationToken);
            }
            catch
            {
                // Không cho audit log làm fail nghiệp vụ chính.
            }
        }
    }
}