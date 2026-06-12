using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Modules.Ordering.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Ordering.Application.Queries
{
    public class GetOrdersByCustomerQuery : IRequest<Result<PagedResultDto<OrderDto>>>
    {
        public string? Keyword { get; set; }

        public Guid CustomerId { get; set; }

        public OrderStatus? Status { get; set; }

        public OrderPaymentStatus? PaymentStatus { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, Result<PagedResultDto<OrderDto>>>
    {
        private readonly IOrderService _orderService;

        public GetOrdersByCustomerQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<Result<PagedResultDto<OrderDto>>> Handle(
            GetOrdersByCustomerQuery request,
            CancellationToken cancellationToken)
        {

            if (request.CustomerId == Guid.Empty)
            {
                return Task.FromResult(Result<PagedResultDto<OrderDto>>.Failure("CustomerId is required."));
            }
            return _orderService.GetCustomerPagedAsync(
                request.CustomerId,
                request.Keyword,
                request.Status,
                request.PaymentStatus,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
