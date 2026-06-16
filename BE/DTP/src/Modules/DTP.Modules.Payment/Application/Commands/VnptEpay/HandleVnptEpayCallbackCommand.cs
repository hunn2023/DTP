using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Commands.VnptEpay
{
    public class HandleVnptEpayCallbackCommand : IRequest<EPayResponse>
    {
        public DTOs.VnptEpayCallbackDto Callback { get; set; } = default!;

        public string RawBody { get; set; } = default!;

        public string? IpAddress { get; set; }
    }

    public class HandleVnptEpayCallbackCommandHandler
    : IRequestHandler<HandleVnptEpayCallbackCommand, EPayResponse>
    {
        private readonly IPaymentService _paymentService;

        public HandleVnptEpayCallbackCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public Task<EPayResponse> Handle(
            HandleVnptEpayCallbackCommand request,
            CancellationToken cancellationToken)
        {
            return _paymentService.HandleVnptEpayCallbackAsync(
                request.Callback,
                request.RawBody,
                request.IpAddress,
                cancellationToken);
        }
    }
}
