using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Commands.Sepay
{
    public sealed record HandleSepayWebhookCommand(
       SepayWebhookDto Callback,
       string RawBody,
       string? Signature,
       string? IpAddress
   ) : IRequest<Result<bool>>;
    public sealed class HandleSepayWebhookCommandHandler
      : IRequestHandler<HandleSepayWebhookCommand, Result<bool>>
    {
        private readonly ISepayPaymentService _paymentService;

        public HandleSepayWebhookCommandHandler(ISepayPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<Result<bool>> Handle(
            HandleSepayWebhookCommand request,
            CancellationToken cancellationToken)
        {
            return await _paymentService.HandleSepayWebhookAsync(
                callback: request.Callback,
                rawBody: request.RawBody,
                signature: request.Signature,
                ipAddress: request.IpAddress,
                cancellationToken: cancellationToken);
        }
    }

}
