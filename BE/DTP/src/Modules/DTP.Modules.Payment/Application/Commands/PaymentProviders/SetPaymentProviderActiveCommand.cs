using DTP.Modules.Payment.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Commands.PaymentProviders
{
    public record SetPaymentProviderActiveCommand(
      Guid Id,
      bool IsActive
  ) : IRequest;

    public class SetPaymentProviderActiveCommandHandler
        : IRequestHandler<SetPaymentProviderActiveCommand>
    {
        private readonly IPaymentProviderService _paymentProviderService;

        public SetPaymentProviderActiveCommandHandler(
            IPaymentProviderService paymentProviderService)
        {
            _paymentProviderService = paymentProviderService;
        }

        public Task Handle(
            SetPaymentProviderActiveCommand request,
            CancellationToken cancellationToken)
        {
            return _paymentProviderService.SetActiveAsync(
                request.Id,
                request.IsActive,
                cancellationToken);
        }
    }
}
