using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Ordering.Application.Commands.MarkOrderPaid
{
    public class MarkOrderPaidCommand : IRequest<Result>
    {
        public Guid OrderId { get; set; }
        public string? Note { get; set; }
    }

    public class MarkOrderPaidCommandHandler : IRequestHandler<MarkOrderPaidCommand, Result>
    {
        private readonly IOrderService  _orderService;

        public MarkOrderPaidCommandHandler(
            
            IOrderService orderService
            )
        {
            _orderService = orderService;
        }

        public async Task<Result> Handle(
            MarkOrderPaidCommand request,
            CancellationToken cancellationToken)
        {
            return await _orderService.MarkPaidAsync(
                request.OrderId,
                request.Note,
                cancellationToken);
        }
    }
}
