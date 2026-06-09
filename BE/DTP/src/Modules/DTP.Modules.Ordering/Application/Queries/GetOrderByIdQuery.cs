using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Ordering.Application.Queries
{
    public class GetOrderByIdQuery : IRequest<Result<OrderDetailDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailDto>>
    {
        private readonly IOrderService _orderService;

        public GetOrderByIdQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<Result<OrderDetailDto>> Handle(
            GetOrderByIdQuery request,
            CancellationToken cancellationToken)
        {
            return _orderService.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
