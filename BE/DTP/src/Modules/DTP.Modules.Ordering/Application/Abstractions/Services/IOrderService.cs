
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Modules.Ordering.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;


namespace DTP.Modules.Ordering.Application.Abstractions.Services
{
    public interface IOrderService
    {
        Task<Result<Guid>> CreateAsync(
               Guid customerId,
               string? customerEmail,
               string? customerPhone,
               string? customerName,
               string currency,
               string? note,
               List<CreateOrderItemRequest> items,
               CancellationToken cancellationToken = default);

        Task<Result> ConfirmAsync(
            Guid orderId,
            string? paymentMethod,
            Guid? changedBy,
            CancellationToken cancellationToken = default);

        Task<Result> MarkPaidAsync(
            Guid orderId,
            string paymentTransactionId,
            Guid? changedBy,
            CancellationToken cancellationToken = default);

        Task<Result> CompleteAsync(
            Guid orderId,
            Guid? changedBy,
            CancellationToken cancellationToken = default);

        Task<Result> CancelAsync(
            Guid orderId,
            string reason,
            Guid? changedBy,
            CancellationToken cancellationToken = default);

        Task<Result<OrderDetailDto>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<PagedResultDto<OrderDto>>> GetPagedAsync(
            string? keyword,
            Guid? customerId,
            OrderStatus? status,
            OrderPaymentStatus? paymentStatus,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);


        //Task<Result<int>> ExpireWaitingPaymentOrdersAsync(
        //        CancellationToken cancellationToken = default);
    }
}
