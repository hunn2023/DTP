using DTP.Modules.Payment.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Commands.PaymentProviders
{
    public record UpdatePaymentProviderLimitsCommand(
      Guid Id,
      decimal? MinAmount,
      decimal? MaxAmount
  ) : IRequest;

    public class UpdatePaymentProviderLimitsCommandHandler
        : IRequestHandler<UpdatePaymentProviderLimitsCommand>
    {
        private readonly IPaymentProviderService _paymentProviderService;

        public UpdatePaymentProviderLimitsCommandHandler(
            IPaymentProviderService paymentProviderService)
        {
            _paymentProviderService = paymentProviderService;
        }

        public Task Handle(
            UpdatePaymentProviderLimitsCommand request,
            CancellationToken cancellationToken)
        {
            return _paymentProviderService.UpdateLimitsAsync(
                request.Id,
                request.MinAmount,
                request.MaxAmount,
                cancellationToken);
        }
    }
}
