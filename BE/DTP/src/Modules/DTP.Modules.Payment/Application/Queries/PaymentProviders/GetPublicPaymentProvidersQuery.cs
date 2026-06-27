using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Queries.PaymentProviders
{
    public record GetPublicPaymentProvidersQuery
      : IRequest<IReadOnlyList<PaymentProviderPublicDto>>;

    public class GetPublicPaymentProvidersQueryHandler
        : IRequestHandler<GetPublicPaymentProvidersQuery, IReadOnlyList<PaymentProviderPublicDto>>
    {
        private readonly IPaymentProviderService _paymentProviderService;

        public GetPublicPaymentProvidersQueryHandler(
            IPaymentProviderService paymentProviderService)
        {
            _paymentProviderService = paymentProviderService;
        }

        public Task<IReadOnlyList<PaymentProviderPublicDto>> Handle(
            GetPublicPaymentProvidersQuery request,
            CancellationToken cancellationToken)
        {
            return _paymentProviderService.GetPublicActiveAsync(cancellationToken);
        }
    }
}
