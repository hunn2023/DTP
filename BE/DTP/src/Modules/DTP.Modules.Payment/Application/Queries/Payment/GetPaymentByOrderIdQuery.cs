using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Queries.Payment
{
    public class GetPaymentByOrderIdQuery : IRequest<PaymentTransactionDto?>
    {
        public Guid OrderId { get; set; }
    }

    public class GetPaymentByOrderIdQueryHandler
       : IRequestHandler<GetPaymentByOrderIdQuery, PaymentTransactionDto?>
    {
        private readonly IPaymentService _paymentService;

        public GetPaymentByOrderIdQueryHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<PaymentTransactionDto?> Handle(
            GetPaymentByOrderIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _paymentService.GetByOrderIdAsync(
                request.OrderId,
                cancellationToken);
        }
    }
}
