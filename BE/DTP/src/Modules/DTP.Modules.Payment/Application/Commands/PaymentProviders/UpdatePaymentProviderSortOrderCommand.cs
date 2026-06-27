using DTP.Modules.Payment.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Commands.PaymentProviders
{
    public record UpdatePaymentProviderSortOrderCommand(
     Guid Id,
     int SortOrder
 ) : IRequest;

    public class UpdatePaymentProviderSortOrderCommandHandler
        : IRequestHandler<UpdatePaymentProviderSortOrderCommand>
    {
        private readonly IPaymentProviderService _paymentProviderService;

        public UpdatePaymentProviderSortOrderCommandHandler(
            IPaymentProviderService paymentProviderService)
        {
            _paymentProviderService = paymentProviderService;
        }

        public Task Handle(
            UpdatePaymentProviderSortOrderCommand request,
            CancellationToken cancellationToken)
        {
            return _paymentProviderService.UpdateSortOrderAsync(
                request.Id,
                request.SortOrder,
                cancellationToken);
        }
    }
}
