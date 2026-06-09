using DTP.Modules.Ordering.Application.Commands.Checkout;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Modules.Ordering.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;


namespace DTP.Modules.Ordering.Application.Abstractions.Services
{
    public interface IOrderService
    {
        Task<Result<CheckoutResultDto>> CheckoutAsync(
            CheckoutCommand request,
            CancellationToken cancellationToken = default);

        Task<Result<OrderDetailDto?>> GetOrderByIdAsync(
            Guid orderId,
            Guid userId,
            bool isAdmin,
            CancellationToken cancellationToken = default);

        Task<Result<PagedResultDto<OrderListItemDto>>> GetMyOrdersAsync(
               Guid userId,
               OrderStatus? status,
               OrderPaymentStatus? paymentStatus,
               int pageIndex,
               int pageSize,
               CancellationToken cancellationToken = default);

        Task<Result<PagedResultDto<OrderListItemDto>>> GetAdminOrdersAsync(
            string? keyword,
            OrderStatus? status,
            OrderPaymentStatus? paymentStatus,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result> MarkPaidAsync(
            Guid orderId,
            string? note,
            CancellationToken cancellationToken = default);

        Task<Result> CancelAsync(
            Guid orderId,
            Guid userId,
            bool isAdmin,
            string? reason,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateStatusAsync(
            Guid orderId,
            OrderStatus status,
            string? note,
            CancellationToken cancellationToken = default);
    }
}
