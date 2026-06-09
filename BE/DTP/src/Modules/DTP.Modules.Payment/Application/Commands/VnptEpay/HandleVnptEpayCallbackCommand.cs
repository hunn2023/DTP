using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Commands.VnptEpay
{
    public class HandleVnptEpayCallbackCommand : IRequest<bool>
    {
        public string? OrderCode { get; set; }

        public string? TransactionCode { get; set; }

        public string? ProviderTransactionCode { get; set; }

        public decimal Amount { get; set; }

        public string? Status { get; set; }

        public string? Message { get; set; }

        public string? Signature { get; set; }

        public string RawBody { get; set; } = default!;
    }

    public class HandleVnptEpayCallbackCommandHandler
       : IRequestHandler<HandleVnptEpayCallbackCommand, bool>
    {
        private readonly IPaymentService _paymentService;

        public HandleVnptEpayCallbackCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<bool> Handle(
            HandleVnptEpayCallbackCommand request,
            CancellationToken cancellationToken)
        {
            return await _paymentService.HandleVnptEpayCallbackAsync(
                new VnptEpayCallbackDto
                {
                    OrderCode = request.OrderCode,
                    TransactionCode = request.TransactionCode,
                    ProviderTransactionCode = request.ProviderTransactionCode,
                    Amount = request.Amount,
                    Status = request.Status,
                    Message = request.Message,
                    Signature = request.Signature,
                    RawBody = request.RawBody
                },
                cancellationToken);
        }
    }
}
