using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Ordering.Application.Commands.Checkout
{
    public class CheckoutCommand : IRequest<Result<CheckoutResultDto>>
    {
        public Guid UserId { get; set; }

        public string CustomerEmail { get; set; } = default!;
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }

        public int Quantity { get; set; } = 1;

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? ReferrerUrl { get; set; }
        public string? CheckoutSource { get; set; }
    }

    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand,
        Result<CheckoutResultDto>>
    {
        private readonly IOrderService _orderService;

        public CheckoutCommandHandler(

            IOrderService orderService
            )
        {
            _orderService = orderService;

        }

        public async Task<Result<CheckoutResultDto>> Handle(
          CheckoutCommand request,
          CancellationToken cancellationToken)
        {
            return await _orderService.CheckoutAsync(request, cancellationToken);
        }
    }
}
