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
using DTP.Shared.Application.Http;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;
using Microsoft.AspNetCore.Http;


namespace DTP.Modules.Ordering.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderHistoryRepository _historyRepository;
        private readonly IOrderUnitOfWork _unitOfWork;
        private readonly IAuditLogWriter _auditLogWriter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRateLimitService _orderRateLimitService;
        private readonly ICacheService _cacheService;
        public OrderService(
            IOrderRepository orderRepository,
            IOrderHistoryRepository historyRepository,
            IOrderUnitOfWork unitOfWork,
            IAuditLogWriter auditLogWriter,
            IHttpContextAccessor httpContextAccessor,
            IOrderRateLimitService orderRateLimitService,
            ICacheService cacheService)
            
        {
            _orderRepository = orderRepository;
            _historyRepository = historyRepository;
            _unitOfWork = unitOfWork;
            _auditLogWriter = auditLogWriter;
            _httpContextAccessor = httpContextAccessor;
            _orderRateLimitService = orderRateLimitService;
            _cacheService = cacheService;
        }

        public async Task<Result<Guid>> CreateAsync(
              Guid customerId,
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

            var ipAddress = GetClientIpAddress();
            var userAgent = GetUserAgent();

            var createBlocked = await _orderRateLimitService.IsCreateOrderBlockedAsync(
                customerId,
                ipAddress,
                cancellationToken);

            if (createBlocked)
            {
                await WriteAuditSafeAsync(
                    action: "Create Order Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "Order",
                    description: "Create order failed because rate limit exceeded.",
                    newValues: new
                    {
                        CustomerId = customerId,
                        CustomerEmail = customerEmail,
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        Reason = "Create order rate limit exceeded",
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: "Bạn tạo đơn hàng quá nhiều lần. Vui lòng thử lại sau.",
                    cancellationToken: cancellationToken);

                return Result<Guid>.Failure(
                    "Bạn tạo đơn hàng quá nhiều lần. Vui lòng thử lại sau.");
            }

            await _orderRateLimitService.RegisterCreateOrderAttemptAsync(
                customerId,
                ipAddress,
                cancellationToken);

            if (customerId != Guid.Empty)
            {
                var pendingCount = await _orderRepository.CountWaitingPaymentOrdersAsync(
                    customerId,
                    cancellationToken);

                if (pendingCount >= 5)
                {
                    await WriteAuditSafeAsync(
                        action: "Create Order Failed",
                        actionType: AuditActionType.Create,
                        status: AuditStatus.Failed,
                        entityName: "Order",
                        description: "Create order failed because customer has too many waiting payment orders.",
                        newValues: new
                        {
                            CustomerId = customerId,
                            CustomerEmail = customerEmail,
                            PendingCount = pendingCount,
                            IpAddress = ipAddress,
                            UserAgent = userAgent,
                            Reason = "Too many waiting payment orders",
                            Request = GetRequestAuditInfo()
                        },
                        errorMessage: "Bạn đang có quá nhiều đơn hàng chờ thanh toán.",
                        cancellationToken: cancellationToken);

                    return Result<Guid>.Failure(
                        "Bạn đang có quá nhiều đơn hàng chờ thanh toán. Vui lòng thanh toán hoặc hủy bớt đơn cũ.");
                }
            }

            currency = string.IsNullOrWhiteSpace(currency)
                ? "VND"
                : currency.Trim().ToUpperInvariant();

            if (currency.Length > 10)
                return Result<Guid>.Failure("Mã tiền tệ không hợp lệ.");

            if (!string.IsNullOrWhiteSpace(customerEmail))
                customerEmail = customerEmail.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(customerPhone))
                customerPhone = customerPhone.Trim();

            if (!string.IsNullOrWhiteSpace(customerName))
                customerName = customerName.Trim();

            if (string.IsNullOrWhiteSpace(customerEmail))
                return Result<Guid>.Failure("Vui lòng nhập email khách hàng.");

            var normalizedItems = new List<CreateOrderItemRequest>();

            foreach (var request in items)
            {
                if (request == null)
                    return Result<Guid>.Failure("Sản phẩm trong đơn hàng không hợp lệ.");

                if (request.ProductId == Guid.Empty)
                    return Result<Guid>.Failure("ProductId không hợp lệ.");

                if (string.IsNullOrWhiteSpace(request.ProductName))
                    return Result<Guid>.Failure("Tên sản phẩm không hợp lệ.");

                if (request.Quantity <= 0)
                    return Result<Guid>.Failure("Số lượng phải lớn hơn 0.");

                if (request.Quantity > 10)
                    return Result<Guid>.Failure("Số lượng mỗi sản phẩm không được vượt quá 10.");

                if (request.UnitPrice < 0)
                    return Result<Guid>.Failure("Đơn giá không hợp lệ.");

                if (request.UnitPrice > 100000000)
                    return Result<Guid>.Failure("Đơn giá vượt quá giới hạn cho phép.");

                normalizedItems.Add(request);
            }

            if (normalizedItems.Count > 20)
                return Result<Guid>.Failure("Một đơn hàng không được vượt quá 20 sản phẩm.");

            var orderCode = GenerateOrderCode();

            var order = new Order(
                orderCode,
                customerId,
                customerEmail,
                customerPhone,
                customerName,
                currency,
                note?.Trim());

            foreach (var request in normalizedItems)
            {
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
                    order.Status,
                    "Tạo đơn hàng.",
                    customerId),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

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

                   Payment = new
                   {
                       order.PaymentExpiredAt
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
                       Module = "Ordering",
                       AntiDos = new
                       {
                           RateLimitChecked = true,
                           PendingOrderChecked = customerId 
                       }
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
            if (orderId == Guid.Empty)
            {
                await WriteAuditSafeAsync(
                    action: "Confirm Order Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Order",
                    description: "Confirm order failed because order id is empty.",
                    newValues: new
                    {
                        OrderId = orderId,
                        PaymentMethod = paymentMethod,
                        ChangedBy = changedBy,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: "OrderId không hợp lệ.",
                    cancellationToken: cancellationToken);

                return Result.Failure("OrderId không hợp lệ.");
            }

            paymentMethod = string.IsNullOrWhiteSpace(paymentMethod)
                ? null
                : paymentMethod.Trim();

            if (!string.IsNullOrWhiteSpace(paymentMethod) && paymentMethod.Length > 50)
            {
                await WriteAuditSafeAsync(
                    action: "Confirm Order Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "Order",
                    entityId: orderId,
                    description: "Confirm order failed because payment method is too long.",
                    newValues: new
                    {
                        OrderId = orderId,
                        PaymentMethod = paymentMethod,
                        ChangedBy = changedBy,
                        Request = GetRequestAuditInfo()
                    },
                    errorMessage: "Phương thức thanh toán không hợp lệ.",
                    cancellationToken: cancellationToken);

                return Result.Failure("Phương thức thanh toán không hợp lệ.");
            }

            var order = await _orderRepository.GetDetailByIdAsync(
                orderId,
                cancellationToken);

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

            if (order.Status == OrderStatus.PendingPayment &&
                order.PaymentStatus == OrderPaymentStatus.Unpaid)
            {
                await WriteAuditSafeAsync(
                    action: "Confirm Order Idempotent",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Success,
                    entityName: "Order",
                    entityId: order.Id,
                    description: $"Order {order.OrderCode} was already confirmed.",
                    oldValues: oldValues,
                    newValues: new
                    {
                        Order = GetOrderSnapshot(order),
                        ChangedBy = changedBy,
                        PaymentMethod = paymentMethod,
                        Request = GetRequestAuditInfo(),
                        Reason = "Order already waiting payment"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Đơn hàng đã được xác nhận trước đó.");
            }

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
                    Payment = new
                    {
                        order.PaymentMethod,
                        order.PaymentStatus,
                        order.PaymentExpiredAt
                    },
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

            if (pageSize > 100)
                pageSize = 100;

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


        public async Task<Result<PagedResultDto<OrderDto>>> GetCustomerPagedAsync(
    Guid customerId,
    string? keyword,
    OrderStatus? status,
    OrderPaymentStatus? paymentStatus,
    int pageIndex,
    int pageSize,
    CancellationToken cancellationToken = default)
        {
            if (customerId == Guid.Empty)
                return Result<PagedResultDto<OrderDto>>.Failure("CustomerId không hợp lệ.");

            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            if (pageSize > 100)
                pageSize = 100;

            keyword = string.IsNullOrWhiteSpace(keyword)
                ? null
                : keyword.Trim();

            var cacheKey = BuildCustomerOrderPagedCacheKey(
                customerId,
                keyword,
                status,
                paymentStatus,
                pageIndex,
                pageSize);

            var cachedResult = await _cacheService.GetAsync<PagedResultDto<OrderDto>>(
                cacheKey,
                cancellationToken);

            if (cachedResult != null)
            {
                await WriteAuditSafeAsync(
                    action: "View Customer Order List From Cache",
                    actionType: AuditActionType.View,
                    status: AuditStatus.Success,
                    entityName: "Order",
                    description: "Customer order list was viewed from cache.",
                    newValues: new
                    {
                        Filter = new
                        {
                            CustomerId = customerId,
                            Keyword = keyword,
                            Status = status?.ToString(),
                            PaymentStatus = paymentStatus?.ToString(),
                            PageIndex = pageIndex,
                            PageSize = pageSize
                        },
                        Result = new
                        {
                            ReturnedCount = cachedResult.Items.Count,
                            TotalCount = cachedResult.TotalCount,
                            Source = "Cache"
                        },
                        Request = GetRequestAuditInfo()
                    },
                    cancellationToken: cancellationToken);

                return Result<PagedResultDto<OrderDto>>.Success(cachedResult);
            }

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

            await _cacheService.SetAsync(
                cacheKey,
                dto,
                TimeSpan.FromMinutes(2),
                cancellationToken);

            await WriteAuditSafeAsync(
                action: "View Customer Order List",
                actionType: AuditActionType.View,
                status: AuditStatus.Success,
                entityName: "Order",
                description: "Customer order list was viewed.",
                newValues: new
                {
                    Filter = new
                    {
                        CustomerId = customerId,
                        Keyword = keyword,
                        Status = status?.ToString(),
                        PaymentStatus = paymentStatus?.ToString(),
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    },
                    Result = new
                    {
                        ReturnedCount = dto.Items.Count,
                        TotalCount = dto.TotalCount,
                        Source = "Database"
                    },
                    Request = GetRequestAuditInfo()
                },
                cancellationToken: cancellationToken);

            return Result<PagedResultDto<OrderDto>>.Success(dto);
        }


        //public async Task<Result<int>> ExpireWaitingPaymentOrdersAsync(
        //    CancellationToken cancellationToken = default)
        //{
        //    var now = DateTime.UtcNow;

        //    var orders = await _orderRepository.GetExpiredWaitingPaymentOrdersAsync(
        //        now,
        //        200,
        //        cancellationToken);

        //    if (orders.Count == 0)
        //        return Result<int>.Success(0);

        //    foreach (var order in orders)
        //    {
        //        var oldStatus = order.Status;

        //        try
        //        {
        //            order.Expire("Đơn hàng hết hạn thanh toán.");

        //            await AddHistoryAsync(
        //                order.Id,
        //                oldStatus,
        //                order.Status,
        //                "Đơn hàng hết hạn thanh toán.",
        //                null,
        //                cancellationToken);

        //            _orderRepository.Update(order);
        //        }
        //        catch
        //        {
        //            // Bỏ qua từng đơn lỗi để không làm dừng toàn bộ batch.
        //        }
        //    }

        //    await _unitOfWork.SaveChangesAsync(cancellationToken);

        //    await WriteAuditSafeAsync(
        //        action: "Expire Waiting Payment Orders",
        //        actionType: AuditActionType.Update,
        //        status: AuditStatus.Success,
        //        entityName: "Order",
        //        description: "Expired waiting payment orders successfully.",
        //        newValues: new
        //        {
        //            Count = orders.Count,
        //            ActionAt = now
        //        },
        //        cancellationToken: cancellationToken);

        //    return Result<int>.Success(orders.Count);
        //}


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
                PaymentExpiredAt = order.PaymentExpiredAt,
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


        private static string BuildCustomerOrderPagedCacheKey(
                Guid customerId,
                string? keyword,
                OrderStatus? status,
                OrderPaymentStatus? paymentStatus,
                int pageIndex,
                int pageSize)
        {
            var normalizedKeyword = string.IsNullOrWhiteSpace(keyword)
                ? "all"
                : keyword.Trim().ToLowerInvariant();

            var normalizedStatus = status?.ToString().ToLowerInvariant() ?? "all";
            var normalizedPaymentStatus = paymentStatus?.ToString().ToLowerInvariant() ?? "all";

            return string.Join(":",
                "ordering",
                "customer-orders",
                customerId,
                normalizedKeyword,
                normalizedStatus,
                normalizedPaymentStatus,
                pageIndex,
                pageSize);
        }


        private object GetRequestAuditInfo()
        {
            return new
            {
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent(),
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
                    order.PaymentStatus,
                    order.PaymentTransactionId,
                    order.PaymentExpiredAt,
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

        private string GetClientIpAddress()
        {
            return _httpContextAccessor.HttpContext.GetClientIp();
        }

        private string? GetUserAgent()
        {
            return _httpContextAccessor.HttpContext.GetUserAgent();
        }
    }
}
