using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Ordering.Application.Queries
{
    public class GetOrderByIdQuery : IRequest<Result<OrderDetailDto?>>
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailDto?>>
    {
        private readonly IOrderService _orderService;

        public GetOrderByIdQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<Result<OrderDetailDto?>> Handle(
             GetOrderByIdQuery request,
             CancellationToken cancellationToken)
            {
            return await _orderService.GetOrderByIdAsync(
                request.OrderId,
                request.UserId,
                request.IsAdmin,
                cancellationToken);
        }
    }
}
