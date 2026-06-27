using DTP.Modules.Payment.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Commands.PaymentProviders
{
    public record SetDefaultPaymentProviderCommand(Guid Id) : IRequest;

    public class SetDefaultPaymentProviderCommandHandler
        : IRequestHandler<SetDefaultPaymentProviderCommand>
    {
        private readonly IPaymentProviderService _paymentProviderService;

        public SetDefaultPaymentProviderCommandHandler(
            IPaymentProviderService paymentProviderService)
        {
            _paymentProviderService = paymentProviderService;
        }

        public Task Handle(
            SetDefaultPaymentProviderCommand request,
            CancellationToken cancellationToken)
        {
            return _paymentProviderService.SetDefaultAsync(
                request.Id,
                cancellationToken);
        }
    }
}
