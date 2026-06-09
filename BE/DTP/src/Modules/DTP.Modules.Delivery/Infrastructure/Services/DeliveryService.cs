using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Domain.Enums;
using DTP.Modules.Delivery.Application.Abstractions.Repositories;
using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Application.DTOs;
using DTP.Modules.Delivery.Domain.Entities;
using DTP.Modules.Delivery.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Application.Delivery;

namespace DTP.Modules.Delivery.Infrastructure.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IDeliveryUnitOfWork _unitOfWork;
        private readonly IOrderDeliveryReader _orderDeliveryReader;
        private readonly IDigitalFulfillmentService _digitalFulfillmentService;
        private readonly IAuditLogWriter _auditLogService;
        private readonly IEsimDeliveryEmailService _esimDeliveryEmailService;

        public DeliveryService(
            IDeliveryRepository deliveryRepository,
            IDeliveryUnitOfWork unitOfWork,
            IOrderDeliveryReader orderDeliveryReader,
            IDigitalFulfillmentService digitalFulfillmentService,
            IAuditLogWriter auditLogService,
            IEsimDeliveryEmailService esimDeliveryEmailService)
        {
            _deliveryRepository = deliveryRepository;
            _unitOfWork = unitOfWork;
            _orderDeliveryReader = orderDeliveryReader;
            _digitalFulfillmentService = digitalFulfillmentService;
            _auditLogService = auditLogService;
            _esimDeliveryEmailService = esimDeliveryEmailService;
        }

        public async Task<Result<Guid>> CreatePendingAsync(
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            if (orderId == Guid.Empty)
                return Result<Guid>.Failure("OrderId không hợp lệ.");

            var exists = await _deliveryRepository.ExistsByOrderIdAsync(
                orderId,
                cancellationToken);

            if (exists)
            {
                var existing = await _deliveryRepository.GetByOrderIdAsync(
                    orderId,
                    cancellationToken);

                return Result<Guid>.Success(existing!.Id);
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
                delivery.AddItem(
                    item.OrderItemId,
                    item.ProductId,
                    item.ProductVariantId,
                    item.ProductName,
                    item.Sku,
                    item.Quantity);
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
            CancellationToken cancellationToken = default)
        {
            if (deliveryId == Guid.Empty)
                return Result.Failure("DeliveryId không hợp lệ.");

            var delivery = await _deliveryRepository.GetByIdAsync(
                deliveryId,
                cancellationToken);

            if (delivery == null)
                return Result.Failure("Không tìm thấy lệnh giao hàng.");

            if (delivery.Status == DeliveryStatus.Delivered)
                return Result.Success();

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
                    delivery.Status,
                    delivery.AttemptCount,
                    ProcessAt = DateTime.UtcNow
                },
                cancellationToken: cancellationToken);

            var fulfillment = await _digitalFulfillmentService.FulfillAsync(
                delivery.OrderId,
                cancellationToken);

            if (!fulfillment.Success)
            {
                var error = string.IsNullOrWhiteSpace(fulfillment.ErrorMessage)
                    ? "Không thể xử lý giao hàng số."
                    : fulfillment.ErrorMessage;

                delivery.MarkFailed(error);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await WriteAuditSafeAsync(
                    action: "Delivery Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Delivery",
                    entityId: delivery.Id,
                    description: "Delivery processing failed.",
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

                return Result.Failure(error);
            }

            foreach (var resultItem in fulfillment.Items)
            {
                var deliveryItem = delivery.Items.FirstOrDefault(x =>
                    x.OrderItemId == resultItem.OrderItemId);

                if (deliveryItem == null)
                    continue;

                delivery.SetItemFulfillment(
                    deliveryItem.Id,
                    resultItem.QrCodeUrl,
                    resultItem.ActivationCode,
                    resultItem.SerialNumber,
                    resultItem.ProviderReference,
                    resultItem.RawData);
            }

            delivery.MarkDelivered("Digital products delivered successfully.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);


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
                    })
                },
                cancellationToken: cancellationToken);

            return Result.Success();
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