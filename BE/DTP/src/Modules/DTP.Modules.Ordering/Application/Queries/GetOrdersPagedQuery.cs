using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Modules.Ordering.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.Queries
{
    public class GetOrdersPagedQuery : IRequest<Result<PagedResultDto<OrderDto>>>
    {
        public string? Keyword { get; set; }

        public Guid? CustomerId { get; set; }

        public OrderStatus? Status { get; set; }

        public OrderPaymentStatus? PaymentStatus { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetOrdersPagedQueryHandler : IRequestHandler<GetOrdersPagedQuery, Result<PagedResultDto<OrderDto>>>
    {
        private readonly IOrderService _orderService;

        public GetOrdersPagedQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<Result<PagedResultDto<OrderDto>>> Handle(
            GetOrdersPagedQuery request,
            CancellationToken cancellationToken)
        {
            return _orderService.GetPagedAsync(
                request.Keyword,
                request.CustomerId,
                request.Status,
                request.PaymentStatus,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
