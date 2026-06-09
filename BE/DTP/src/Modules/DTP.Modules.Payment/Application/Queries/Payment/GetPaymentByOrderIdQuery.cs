using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Payment.Application.Queries.Payment
{

    public class GetPaymentByOrderIdQuery : IRequest<Result<PaymentTransactionDto>>
    {
        public Guid OrderId { get; set; }
    }

    public class GetPaymentByOrderIdQueryHandler
        : IRequestHandler<GetPaymentByOrderIdQuery, Result<PaymentTransactionDto>>
    {
        private readonly IPaymentService _paymentService;

        public GetPaymentByOrderIdQueryHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public Task<Result<PaymentTransactionDto>> Handle(
            GetPaymentByOrderIdQuery request,
            CancellationToken cancellationToken)
        {
            return _paymentService.GetByOrderIdAsync(
                request.OrderId,
                cancellationToken);
        }
    }
}
