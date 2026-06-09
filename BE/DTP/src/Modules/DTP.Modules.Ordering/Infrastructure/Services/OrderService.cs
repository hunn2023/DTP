using Azure.Core;
using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Application.Services;
using DTP.Modules.Audit.Domain.Enums;
using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Modules.Ordering.Domain.Entities;
using DTP.Modules.Ordering.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using Microsoft.AspNetCore.Http;
using System.Net;


namespace DTP.Modules.Ordering.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderHistoryRepository _historyRepository;
        private readonly IOrderUnitOfWork _unitOfWork;
        private readonly IAuditLogWriter _auditLogWriter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderService(
            IOrderRepository orderRepository,
            IOrderHistoryRepository historyRepository,
            IOrderUnitOfWork unitOfWork,
            IAuditLogWriter auditLogWriter,
            IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _historyRepository = historyRepository;
            _unitOfWork = unitOfWork;
            _auditLogWriter = auditLogWriter;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<Guid>> CreateAsync(
            Guid? customerId,
            string? customerEmail,
            string? customerPhone,
            string? customerName,
            string currency,
            string? note,
            List<CreateOrderItemRequest> items,
            CancellationToken cancellationToken = default)
        {
            if (items == null || items.Count == 0)
                return Result<Guid>.Failure("Đơn hàng chưa có sản phẩm.");

            currency = string.IsNullOrWhiteSpace(currency)
                ? "VND"
                : currency.Trim().ToUpper();

            var orderCode = GenerateOrderCode();

            var order = new Order(
                orderCode,
                customerId,
                customerEmail?.Trim(),
                customerPhone?.Trim(),
                customerName?.Trim(),
                currency,
                note);

            foreach (var request in items)
            {
                if (request.ProductId == Guid.Empty)
                    return Result<Guid>.Failure("ProductId không hợp lệ.");

                if (string.IsNullOrWhiteSpace(request.ProductName))
                    return Result<Guid>.Failure("Tên sản phẩm không hợp lệ.");

                if (request.Quantity <= 0)
                    return Result<Guid>.Failure("Số lượng phải lớn hơn 0.");

                if (request.UnitPrice < 0)
                    return Result<Guid>.Failure("Đơn giá không hợp lệ.");

                var item = new OrderItem(
                    order.Id,
                    request.ItemType,
                    request.ProductId,
                    request.ProductVariantId,
                    request.EsimPackageId,
                    request.PhoneCardId,
                    request.ProductName.Trim(),
                    request.VariantName?.Trim(),
                    request.Sku?.Trim(),
                    request.Quantity,
                    request.UnitPrice,
                    currency);

                order.AddItem(item);
            }

            await _orderRepository.AddAsync(order, cancellationToken);

            await _historyRepository.AddAsync(
                new OrderHistory(
                    order.Id,
                    OrderStatus.Draft,
                    OrderStatus.Draft,
                    "Tạo đơn hàng.",
                    customerId),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var ipAddress = GetClientIpAddress();
            var userAgent = GetUserAgent();
            var requestPath = GetRequestPath();
            var requestMethod = GetRequestMethod();

            await WriteAuditSafeAsync(
               action: "Create Order",
               actionType: AuditActionType.Create,
               status: AuditStatus.Success,
               entityName: "Order",
               entityId: order.Id,
               description: $"Order {order.OrderCode} was created successfully.",
               newValues: new
               {
                   order.Id,
                   order.OrderCode,

                   Customer = new
                   {
                       order.CustomerId,
                       order.CustomerEmail,
                       order.CustomerPhone,
                       order.CustomerName
                   },

                   Amount = new
                   {
                       order.Currency,
                       order.SubTotal,
                       order.DiscountAmount,
                       order.TotalAmount
                   },

                   Status = new
                   {
                       OrderStatus = order.Status.ToString(),
                       PaymentStatus = order.PaymentStatus.ToString()
                   },

                   Request = GetRequestAuditInfo(),

                   ItemsSummary = new
                   {
                       TotalItems = order.Items.Count,
                       TotalQuantity = order.Items.Sum(x => x.Quantity),
                       EsimCount = order.Items.Count(x => x.ItemType == OrderItemType.Esim),
                       PhoneCardCount = order.Items.Count(x => x.ItemType == OrderItemType.PhoneCard)
                   },

                   Items = order.Items.Select(x => new
                   {
                       x.Id,
                       ItemType = x.ItemType.ToString(),
                       x.ProductId,
                       x.ProductVariantId,
                       x.EsimPackageId,
                       x.PhoneCardId,
                       x.ProductName,
                       x.VariantName,
                       x.Sku,
                       x.Quantity,
                       x.UnitPrice,
                       x.TotalPrice,
                       x.Currency
                   }).ToList(),

                   Metadata = new
                   {
                       Source = "PublicCheckout",
                       Module = "Ordering"
                   }
               },
               cancellationToken: cancellationToken);

            return Result<Guid>.Success(order.Id);
        }

        public async Task<Result> ConfirmAsync(
            Guid orderId,
            string? paymentMethod,
            Guid? changedBy,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetDetailByIdAsync(orderId, cancellationToken);

            if (order == null)
            {
                await WriteAuditSafeAsync(
                       action: "Confirm Order Failed",
                       actionType: AuditActionType.Update,
                       status: AuditStatus.Failed,
                       entityName: "Order",
                       entityId: orderId,
                       description: "Confirm order failed because order was not found.",
                       newValues: new
                       {
                           OrderId = orderId,
                           PaymentMethod = paymentMethod,
                           ChangedBy = changedBy,
                           Request = GetRequestAuditInfo()
                       },
                       errorMessage: "Không tìm thấy đơn hàng.",
                       cancellationToken: cancellationToken);

                return Result.Failure("Không tìm thấy đơn hàng.");
            }
                
            var oldStatus = order.Status;
            var oldValues = GetOrderSnapshot(order);

            try
            {
                order.Confirm(paymentMethod);
            }
            catch (Exception ex)
            {
                await WriteAuditSafeAsync(
                   action: "Confirm Order Failed",
                   actionType: AuditActionType.Update,
                   status: AuditStatus.Failed,
                   entityName: "Order",
                   entityId: order.Id,
                   description: $"Confirm order {order.OrderCode} failed.",
                   oldValues: oldValues,
                   newValues: new
                   {
                       PaymentMethod = paymentMethod,
                       ChangedBy = changedBy,
                       Request = GetRequestAuditInfo()
                   },
                   errorMessage: ex.Message,
                   cancellationToken: cancellationToken);
                        return Result.Failure(ex.Message);
            }

            await AddHistoryAsync(
                order.Id,
                oldStatus,
                order.Status,
                "Xác nhận đơn hàng, chờ thanh toán.",
                changedBy,
                cancellationToken);

            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            await WriteAuditSafeAsync(
                   action: "Confirm Order",
                   actionType: AuditActionType.Update,
                   status: AuditStatus.Success,
                   entityName: "Order",
                   entityId: order.Id,
                   description: $"Order {order.OrderCode} was confirmed successfully.",
                   oldValues: oldValues,
                   newValues: new
                   {
                       Order = GetOrderSnapshot(order),
                       ChangedBy = changedBy,
                       PaymentMethod = paymentMethod,
                       Request = GetRequestAuditInfo()
                   },
                   cancellationToken: cancellationToken);

            return Result.Success();
        }

        public async Task<Result> MarkPaidAsync(
    Guid orderId,
    string paymentTransactionId,
    Guid? changedBy,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(paymentTransactionId))
            {
                await WriteAuditSafeAsync(
                    action: "Mark Order Paid Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Order",
                    entityId: orderId,
                    description: "Mark order paid failed because payment transaction id is empty.",
                    newValues: new
                    {
                        OrderId = orderId,
                        PaymentTransactionId = paymentTransactionId,
                        ChangedBy = changedBy,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: "Mã giao dịch thanh toán không hợp lệ.",
                    cancellationToken: cancellationToken);

                return Result.Failure("Mã giao dịch thanh toán không hợp lệ.");
            }

            var order = await _orderRepository.GetDetailByIdAsync(orderId, cancellationToken);

            if (order == null)
            {
                await WriteAuditSafeAsync(
                    action: "Mark Order Paid Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Order",
                    entityId: orderId,
                    description: "Mark order paid failed because order was not found.",
                    newValues: new
                    {
                        OrderId = orderId,
                        PaymentTransactionId = paymentTransactionId,
                        ChangedBy = changedBy,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: "Không tìm thấy đơn hàng.",
                    cancellationToken: cancellationToken);

                return Result.Failure("Không tìm thấy đơn hàng.");
            }

            var oldStatus = order.Status;
            var oldValues = GetOrderSnapshot(order);

            try
            {
                order.MarkPaid(paymentTransactionId.Trim());
            }
            catch (Exception ex)
            {
                await WriteAuditSafeAsync(
                    action: "Mark Order Paid Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Order",
                    entityId: order.Id,
                    description: $"Mark order {order.OrderCode} paid failed.",
                    oldValues: oldValues,
                    newValues: new
                    {
                        PaymentTransactionId = paymentTransactionId,
                        ChangedBy = changedBy,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: ex.Message,
                    cancellationToken: cancellationToken);

                return Result.Failure(ex.Message);
            }

            await AddHistoryAsync(
                order.Id,
                oldStatus,
                order.Status,
                "Thanh toán thành công.",
                changedBy,
                cancellationToken);

            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await WriteAuditSafeAsync(
                action: "Mark Order Paid",
                actionType: AuditActionType.Update,
                status: AuditStatus.Success,
                entityName: "Order",
                entityId: order.Id,
                description: $"Order {order.OrderCode} was marked as paid successfully.",
                oldValues: oldValues,
                newValues: new
                {
                    Order = GetOrderSnapshot(order),
                    ChangedBy = changedBy,
                    PaymentTransactionId = paymentTransactionId,
                    Request = GetRequestAuditInfo()
                },
                cancellationToken: cancellationToken);

            return Result.Success();
        }

        public async Task<Result> CompleteAsync(
    Guid orderId,
    Guid? changedBy,
    CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetDetailByIdAsync(orderId, cancellationToken);

            if (order == null)
            {
                await WriteAuditSafeAsync(
                    action: "Complete Order Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Order",
                    entityId: orderId,
                    description: "Complete order failed because order was not found.",
                    newValues: new
                    {
                        OrderId = orderId,
                        ChangedBy = changedBy,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: "Không tìm thấy đơn hàng.",
                    cancellationToken: cancellationToken);

                return Result.Failure("Không tìm thấy đơn hàng.");
            }

            var oldStatus = order.Status;
            var oldValues = GetOrderSnapshot(order);

            try
            {
                order.Complete();
            }
            catch (Exception ex)
            {
                await WriteAuditSafeAsync(
                    action: "Complete Order Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Order",
                    entityId: order.Id,
                    description: $"Complete order {order.OrderCode} failed.",
                    oldValues: oldValues,
                    newValues: new
                    {
                        ChangedBy = changedBy,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: ex.Message,
                    cancellationToken: cancellationToken);

                return Result.Failure(ex.Message);
            }

            await AddHistoryAsync(
                order.Id,
                oldStatus,
                order.Status,
                "Hoàn tất đơn hàng.",
                changedBy,
                cancellationToken);

            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await WriteAuditSafeAsync(
                action: "Complete Order",
                actionType: AuditActionType.Update,
                status: AuditStatus.Success,
                entityName: "Order",
                entityId: order.Id,
                description: $"Order {order.OrderCode} was completed successfully.",
                oldValues: oldValues,
                newValues: new
                {
                    Order = GetOrderSnapshot(order),
                    ChangedBy = changedBy,
                    Request = GetRequestAuditInfo()
                },
                cancellationToken: cancellationToken);

            return Result.Success();
        }
        public async Task<Result> CancelAsync(
    Guid orderId,
    string reason,
    Guid? changedBy,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                await WriteAuditSafeAsync(
                    action: "Cancel Order Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Order",
                    entityId: orderId,
                    description: "Cancel order failed because cancel reason is empty.",
                    newValues: new
                    {
                        OrderId = orderId,
                        Reason = reason,
                        ChangedBy = changedBy,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: "Vui lòng nhập lý do huỷ đơn.",
                    cancellationToken: cancellationToken);

                return Result.Failure("Vui lòng nhập lý do huỷ đơn.");
            }

            var order = await _orderRepository.GetDetailByIdAsync(orderId, cancellationToken);

            if (order == null)
            {
                await WriteAuditSafeAsync(
                    action: "Cancel Order Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Order",
                    entityId: orderId,
                    description: "Cancel order failed because order was not found.",
                    newValues: new
                    {
                        OrderId = orderId,
                        Reason = reason,
                        ChangedBy = changedBy,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: "Không tìm thấy đơn hàng.",
                    cancellationToken: cancellationToken);

                return Result.Failure("Không tìm thấy đơn hàng.");
            }

            var oldStatus = order.Status;
            var oldValues = GetOrderSnapshot(order);

            try
            {
                order.Cancel(reason.Trim());
            }
            catch (Exception ex)
            {
                await WriteAuditSafeAsync(
                    action: "Cancel Order Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Order",
                    entityId: order.Id,
                    description: $"Cancel order {order.OrderCode} failed.",
                    oldValues: oldValues,
                    newValues: new
                    {
                        Reason = reason,
                        ChangedBy = changedBy,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: ex.Message,
                    cancellationToken: cancellationToken);

                return Result.Failure(ex.Message);
            }

            await AddHistoryAsync(
                order.Id,
                oldStatus,
                order.Status,
                reason,
                changedBy,
                cancellationToken);

            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await WriteAuditSafeAsync(
                action: "Cancel Order",
                actionType: AuditActionType.Update,
                status: AuditStatus.Success,
                entityName: "Order",
                entityId: order.Id,
                description: $"Order {order.OrderCode} was cancelled successfully.",
                oldValues: oldValues,
                newValues: new
                {
                    Order = GetOrderSnapshot(order),
                    Reason = reason,
                    ChangedBy = changedBy,
                    Request = GetRequestAuditInfo()
                },
                cancellationToken: cancellationToken);

            return Result.Success();
        }

        public async Task<Result<OrderDetailDto>> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetDetailByIdAsync(id, cancellationToken);

            if (order == null)
            {
                await WriteAuditSafeAsync(
                    action: "View Order Detail Failed",
                    actionType: AuditActionType.View,
                    status: AuditStatus.Failed,
                    entityName: "Order",
                    entityId: id,
                    description: "View order detail failed because order was not found.",
                    newValues: new
                    {
                        OrderId = id,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: "Không tìm thấy đơn hàng.",
                    cancellationToken: cancellationToken);

                return Result<OrderDetailDto>.Failure("Không tìm thấy đơn hàng.");
            }

            await WriteAuditSafeAsync(
                action: "View Order Detail",
                actionType: AuditActionType.View,
                status: AuditStatus.Success,
                entityName: "Order",
                entityId: order.Id,
                description: $"Order {order.OrderCode} detail was viewed.",
                newValues: new
                {
                    order.Id,
                    order.OrderCode,
                    order.CustomerId,
                    order.CustomerEmail,
                    order.CustomerPhone,
                    order.CustomerName,
                    order.TotalAmount,
                    order.Currency,
                    Status = order.Status.ToString(),
                    PaymentStatus = order.PaymentStatus.ToString(),
                    Request = GetRequestAuditInfo()
                },
                cancellationToken: cancellationToken);

            return Result<OrderDetailDto>.Success(MapDetail(order));
        }

        public async Task<Result<PagedResultDto<OrderDto>>> GetPagedAsync(
     string? keyword,
     Guid? customerId,
     OrderStatus? status,
     OrderPaymentStatus? paymentStatus,
     int pageIndex,
     int pageSize,
     CancellationToken cancellationToken = default)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var result = await _orderRepository.GetPagedAsync(
                keyword,
                customerId,
                status,
                paymentStatus,
                pageIndex,
                pageSize,
                cancellationToken);

            var dto = new PagedResultDto<OrderDto>
            {
                Items = result.Items.Select(Map).ToList(),
                TotalCount = result.Total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            await WriteAuditSafeAsync(
                action: "View Order List",
                actionType: AuditActionType.View,
                status: AuditStatus.Success,
                entityName: "Order",
                description: "Order list was viewed.",
                newValues: new
                {
                    Filter = new
                    {
                        Keyword = keyword,
                        CustomerId = customerId,
                        Status = status?.ToString(),
                        PaymentStatus = paymentStatus?.ToString(),
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    },
                    Result = new
                    {
                        ReturnedCount = dto.Items.Count,
                        TotalCount = dto.TotalCount
                    },
                    Request = GetRequestAuditInfo()
                },
                cancellationToken: cancellationToken);

            return Result<PagedResultDto<OrderDto>>.Success(dto);
        }

        private async Task AddHistoryAsync(
            Guid orderId,
            OrderStatus fromStatus,
            OrderStatus toStatus,
            string? note,
            Guid? changedBy,
            CancellationToken cancellationToken)
        {
            var history = new OrderHistory(
                orderId,
                fromStatus,
                toStatus,
                note,
                changedBy);

            await _historyRepository.AddAsync(history, cancellationToken);
        }

        private static string GenerateOrderCode()
        {
            return $"DTP{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }

        private static OrderDto Map(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CustomerId = order.CustomerId,
                CustomerEmail = order.CustomerEmail,
                CustomerPhone = order.CustomerPhone,
                CustomerName = order.CustomerName,
                Currency = order.Currency,
                SubTotal = order.SubTotal,
                DiscountAmount = order.DiscountAmount,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,
                CreatedAt = order.CreatedAt,
                PaidAt = order.PaidAt
            };
        }


        private async Task WriteAuditSafeAsync(
             string action,
             AuditActionType actionType,
             AuditStatus status,
             string? entityName = null,
             Guid? entityId = null,
             string? description = null,
             object? oldValues = null,
             object? newValues = null,
             string? errorMessage = null,
             CancellationToken cancellationToken = default)
        {
            try
            {
                await _auditLogWriter.WriteAsync(
                    module: "Ordering",
                    action: action,
                    actionType: actionType,
                    status: status,
                    entityName: entityName,
                    entityId: entityId,
                    description: description,
                    oldValues: oldValues,
                    newValues: newValues,
                    errorMessage: errorMessage,
                    cancellationToken: cancellationToken);
            }
            catch
            {
                // Không được để lỗi audit làm fail nghiệp vụ Order.
            }
        }

        private static OrderDetailDto MapDetail(Order order)
        {
            return new OrderDetailDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CustomerId = order.CustomerId,
                CustomerEmail = order.CustomerEmail,
                CustomerPhone = order.CustomerPhone,
                CustomerName = order.CustomerName,
                Currency = order.Currency,
                SubTotal = order.SubTotal,
                DiscountAmount = order.DiscountAmount,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,
                PaymentTransactionId = order.PaymentTransactionId,
                Note = order.Note,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                PaidAt = order.PaidAt,
                CancelledAt = order.CancelledAt,
                CancelReason = order.CancelReason,

                Items = order.Items.Select(x => new OrderItemDto
                {
                    Id = x.Id,
                    ItemType = x.ItemType,
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    EsimPackageId = x.EsimPackageId,
                    PhoneCardId = x.PhoneCardId,
                    ProductName = x.ProductName,
                    VariantName = x.VariantName,
                    Sku = x.Sku,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    TotalPrice = x.TotalPrice,
                    Currency = x.Currency
                }).ToList(),

                Histories = order.Histories
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new OrderHistoryDto
                    {
                        Id = x.Id,
                        FromStatus = x.FromStatus,
                        ToStatus = x.ToStatus,
                        Note = x.Note,
                        ChangedBy = x.ChangedBy,
                        CreatedAt = x.CreatedAt
                    }).ToList()
            };
        }


        private object GetRequestAuditInfo()
        {
            return new
            {
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent(),
                Path = GetRequestPath(),
                Method = GetRequestMethod(),
                ActionAt = DateTime.UtcNow
            };
        }

        private static object GetOrderSnapshot(Order order)
        {
            return new
            {
                order.Id,
                order.OrderCode,

                Customer = new
                {
                    order.CustomerId,
                    order.CustomerEmail,
                    order.CustomerPhone,
                    order.CustomerName
                },

                Amount = new
                {
                    order.Currency,
                    order.SubTotal,
                    order.DiscountAmount,
                    order.TotalAmount
                },

                Status = new
                {
                    OrderStatus = order.Status.ToString(),
                    PaymentStatus = order.PaymentStatus.ToString()
                },

                Payment = new
                {
                    order.PaymentMethod,
                    order.PaymentTransactionId,
                    order.PaidAt
                },

                Cancel = new
                {
                    order.CancelledAt,
                    order.CancelReason
                },

                ItemsSummary = new
                {
                    TotalItems = order.Items.Count,
                    TotalQuantity = order.Items.Sum(x => x.Quantity),
                    EsimCount = order.Items.Count(x => x.ItemType == OrderItemType.Esim),
                    PhoneCardCount = order.Items.Count(x => x.ItemType == OrderItemType.PhoneCard)
                }
            };
        }

        private string? GetClientIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                return null;

            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                return forwardedFor
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault()
                    ?.Trim();
            }

            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(realIp))
                return realIp.Trim();

            return httpContext.Connection.RemoteIpAddress?.ToString();
        }

        private string? GetUserAgent()
        {
            return _httpContextAccessor.HttpContext?
                .Request
                .Headers["User-Agent"]
                .FirstOrDefault();
        }

        private string? GetRequestPath()
        {
            return _httpContextAccessor.HttpContext?
                .Request
                .Path
                .Value;
        }

        private string? GetRequestMethod()
        {
            return _httpContextAccessor.HttpContext?
                .Request
                .Method;
        }
    }
}
