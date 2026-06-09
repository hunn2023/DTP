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
    public class GetPaymentByIdQuery : IRequest<Result<PaymentTransactionDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetPaymentByIdQueryHandler
        : IRequestHandler<GetPaymentByIdQuery, Result<PaymentTransactionDto>>
    {
        private readonly IPaymentService _paymentService;

        public GetPaymentByIdQueryHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public Task<Result<PaymentTransactionDto>> Handle(
            GetPaymentByIdQuery request,
            CancellationToken cancellationToken)
        {
            return _paymentService.GetByIdAsync(
                request.Id,
                cancellationToken);
        }
    }
}
