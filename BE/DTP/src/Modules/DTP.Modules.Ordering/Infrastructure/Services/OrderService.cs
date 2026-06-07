using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.Commands.Checkout;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Modules.Ordering.Domain.Entities;
using DTP.Modules.Ordering.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Ordering.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderUnitOfWork _unitOfWork;
        private readonly IOrderingCatalogService _catalogService;
        private readonly IOrderingPaymentService _paymentService;
        private readonly IOrderCodeGenerator _orderCodeGenerator;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderUnitOfWork unitOfWork,
            IOrderingCatalogService catalogService,
            IOrderingPaymentService paymentService,
            IOrderCodeGenerator orderCodeGenerator)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _catalogService = catalogService;
            _paymentService = paymentService;
            _orderCodeGenerator = orderCodeGenerator;
        }

        public async Task<Result<CheckoutResultDto>> CheckoutAsync(
            CheckoutCommand request,
            CancellationToken cancellationToken = default)
        {
            if (request.UserId == Guid.Empty)
                return Result<CheckoutResultDto>.Failure("User is required.");


            if (string.IsNullOrWhiteSpace(request.CustomerEmail))
                return Result<CheckoutResultDto>.Failure("Customer email is required.");

            if (request.Quantity <= 0)
                return Result<CheckoutResultDto>.Failure("Quantity must be greater than zero.");

            var product = await _catalogService.GetProductForCheckoutAsync(
                request.ProductId,
                request.ProductVariantId,
                cancellationToken);

            if (product == null)
                return Result<CheckoutResultDto>.Failure("Product not found.");

            if (!product.IsActive)
                return Result<CheckoutResultDto>.Failure("Product is not active.");`

            var orderCode = _orderCodeGenerator.Generate();

            var order = new Order(
                orderCode,
                request.UserId,
                request.CustomerEmail,
                request.CustomerName,
                request.CustomerPhone,
                product.CurrencyCode,
                request.IpAddress,
                request.UserAgent,
                request.ReferrerUrl,
                request.CheckoutSource);

            var orderItem = new OrderItem(
                order.Id,
                product.ProductId,
                product.ProductVariantId,
                product.EsimPackageId,
                product.PhoneCardId,
                product.ProductCode,
                product.ProductName,
                product.ProductSlug,
                product.VariantName,
                product.Sku,
                product.ThumbnailUrl,
                request.Quantity,
                product.UnitPrice,
                product.CurrencyCode);

            order.AddItem(orderItem);

            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var payment = await _paymentService.CreatePaymentAsync(
                order.Id,
                order.OrderCode,
                order.TotalAmount,
                order.CurrencyCode,
                order.CustomerEmail,
                cancellationToken);

            order.AttachPaymentTransaction(payment.PaymentTransactionCode);

            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CheckoutResultDto>.Success(new CheckoutResultDto
            {
                OrderId = order.Id,
                OrderCode = order.OrderCode,
                Amount = order.TotalAmount,
                CurrencyCode = order.CurrencyCode,
                PaymentTransactionCode = payment.PaymentTransactionCode,
                PaymentUrl = payment.PaymentUrl,
                QrCodeUrl = payment.QrCodeUrl,
                PaymentExpiredAt = payment.ExpiredAt
            });
        }

        public async Task<Result<OrderDetailDto?>> GetOrderByIdAsync(
            Guid orderId,
            Guid userId,
            bool isAdmin,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetDetailByIdAsync(orderId, cancellationToken);

            if (order == null) return Result<OrderDetailDto?>.Failure("Order not found.");

            if (!isAdmin && order.UserId != userId)
                return Result<OrderDetailDto?>.Failure("You cannot access this order.");

            return Result<OrderDetailDto?>.Success(MapToDetailDto(order));
        }

        public async Task<Result<PagedResultDto<OrderListItemDto>>> GetMyOrdersAsync(
             Guid userId,
             OrderStatus? status,
             OrderPaymentStatus? paymentStatus,
             int pageIndex,
             int pageSize,
             CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
                return Result<PagedResultDto<OrderListItemDto>>.Failure("User is required.");

            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 20;

            if (pageSize > 100)
                pageSize = 100;

            var query = _orderRepository.Query()
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.UserId == userId);

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (paymentStatus.HasValue)
            {
                query = query.Where(x => x.PaymentStatus == paymentStatus.Value);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new OrderListItemDto
                {
                    Id = x.Id,
                    OrderCode = x.OrderCode,
                    UserId = x.UserId,

                    CustomerEmail = x.CustomerEmail,
                    CustomerName = x.CustomerName,
                    CustomerPhone = x.CustomerPhone,

                    TotalAmount = x.TotalAmount,
                    CurrencyCode = x.CurrencyCode,

                    Status = x.Status,
                    StatusName = x.Status.ToString(),

                    PaymentStatus = x.PaymentStatus,
                    PaymentStatusName = x.PaymentStatus.ToString(),

                    CreatedAt = x.CreatedAt,
                    PaidAt = x.PaidAt,

                    PaymentTransactionCode = x.PaymentTransactionCode,

                    IpAddress = x.IpAddress,
                    CheckoutSource = x.CheckoutSource
                })
                .ToListAsync(cancellationToken);


            return Result<PagedResultDto<OrderListItemDto>>.Success(new PagedResultDto<OrderListItemDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            });
        }

        public async Task<Result<PagedResultDto<OrderListItemDto>>> GetAdminOrdersAsync(
            string? keyword,
            OrderStatus? status,
            OrderPaymentStatus? paymentStatus,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 20;

            var query = _orderRepository.Query()
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.OrderCode.Contains(keyword) ||
                    x.CustomerEmail.Contains(keyword) ||
                    x.CustomerName!.Contains(keyword) ||
                    x.CustomerPhone!.Contains(keyword));
            }

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            if (paymentStatus.HasValue)
                query = query.Where(x => x.PaymentStatus == paymentStatus.Value);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new OrderListItemDto
                {
                    Id = x.Id,
                    OrderCode = x.OrderCode,
                    CustomerEmail = x.CustomerEmail,
                    CustomerName = x.CustomerName,
                    TotalAmount = x.TotalAmount,
                    CurrencyCode = x.CurrencyCode,
                    Status = x.Status,
                    StatusName = x.Status.ToString(),
                    PaymentStatus = x.PaymentStatus,
                    PaymentStatusName = x.PaymentStatus.ToString(),
                    CreatedAt = x.CreatedAt,
                    PaidAt = x.PaidAt
                })
                .ToListAsync(cancellationToken);

            return Result<PagedResultDto<OrderListItemDto>>.Success(new PagedResultDto<OrderListItemDto>
            {
                Items = items,
                TotalCount = total,
                PageIndex = pageIndex,
                PageSize = pageSize
            });

        }

        public async Task<Result> MarkPaidAsync(
            Guid orderId,
            string? note,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetDetailByIdAsync(orderId, cancellationToken);

            if (order == null) return Result.Failure("Order not found.");

            order.MarkPaid(note);

            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> CancelAsync(
              Guid orderId,
              Guid userId,
              bool isAdmin,
              string? reason,
              CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetDetailByIdAsync(orderId, cancellationToken);

            if (order == null)
                return Result.Failure("Order not found.");

            if (!isAdmin && order.UserId != userId)
                return Result.Failure("You cannot cancel this order.");

            if (order.PaymentStatus == OrderPaymentStatus.Paid)
                return Result.Failure("Paid order cannot be cancelled.");

            if (order.Status == OrderStatus.Delivered ||
                order.Status == OrderStatus.Completed)
            {
                return Result.Failure("Delivered or completed order cannot be cancelled.");
            }

            order.Cancel(reason ?? "Order cancelled");

            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        public async Task<Result> UpdateStatusAsync(
            Guid orderId,
            OrderStatus status,
            string? note,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetDetailByIdAsync(orderId, cancellationToken);

            if (order == null) return Result.Failure("Order not found.");

            switch (status)
            {
                case OrderStatus.Processing:
                    order.MarkProcessing(note);
                    break;

                case OrderStatus.Delivered:
                    order.MarkDelivered(note);
                    break;

                case OrderStatus.Completed:
                    order.MarkCompleted(note);
                    break;

                case OrderStatus.Cancelled:
                    order.Cancel(note);
                    break;

                case OrderStatus.PaymentFailed:
                    order.MarkPaymentFailed(note);
                    break;

                default:
                    return Result.Failure("Invalid status.");
            }

            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }

        private static OrderDetailDto MapToDetailDto(Order order)
        {
            return new OrderDetailDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                UserId = order.UserId,
                CustomerEmail = order.CustomerEmail,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                CurrencyCode = order.CurrencyCode,
                SubtotalAmount = order.SubtotalAmount,
                DiscountAmount = order.DiscountAmount,
                TotalAmount = order.TotalAmount,
                Status = (int)order.Status,
                StatusName = order.Status.ToString(),
                PaymentStatus = (int)order.PaymentStatus,
                PaymentStatusName = order.PaymentStatus.ToString(),
                CreatedAt = order.CreatedAt,
                PaidAt = order.PaidAt,
                IpAddress = order.IpAddress,
                UserAgent = order.UserAgent,
                ReferrerUrl = order.ReferrerUrl,
                CheckoutSource = order.CheckoutSource,
                Items = order.Items.Select(x => new OrderItemDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    ProductName = x.ProductName,
                    VariantName = x.VariantName,
                    Sku = x.Sku,
                    ThumbnailUrl = x.ThumbnailUrl,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    TotalPrice = x.TotalPrice,
                    CurrencyCode = x.CurrencyCode
                }).ToList()
            };
        }
    }
}
