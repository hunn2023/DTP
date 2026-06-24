using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Queries.Payment
{
    public sealed record GetPaymentOrderStatusQuery(Guid OrderId)
      : IRequest<Result<PaymentOrderStatusDto>>;


    public sealed class GetPaymentOrderStatusQueryHandler
        : IRequestHandler<GetPaymentOrderStatusQuery, Result<PaymentOrderStatusDto>>
    {
        private readonly IPaymentService _paymentService;

        public GetPaymentOrderStatusQueryHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public Task<Result<PaymentOrderStatusDto>> Handle(
            GetPaymentOrderStatusQuery request,
            CancellationToken cancellationToken)
        {
            return _paymentService.GetOrderPaymentStatusAsync(
                request.OrderId,
                cancellationToken);
        }
    }
}
