using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Modules.Ordering.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Ordering.Application.Queries
{
    public class GetMyOrdersQuery : IRequest<Result<PagedResultDto<OrderListItemDto>>>
    {
        public Guid UserId { get; set; }

        public OrderStatus? Status { get; set; }

        public OrderPaymentStatus? PaymentStatus { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetMyOrdersQueryHandler
       : IRequestHandler<GetMyOrdersQuery, Result<PagedResultDto<OrderListItemDto>>>
    {
        private readonly IOrderService _orderService;

        public GetMyOrdersQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<Result<PagedResultDto<OrderListItemDto>>> Handle(
            GetMyOrdersQuery request,
            CancellationToken cancellationToken)
        {
            return await _orderService.GetMyOrdersAsync(
                request.UserId,
                request.Status,
                request.PaymentStatus,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
