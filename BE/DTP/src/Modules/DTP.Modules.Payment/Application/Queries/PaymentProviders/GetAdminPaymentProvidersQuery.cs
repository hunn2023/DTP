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
    public record GetAdminPaymentProvidersQuery
     : IRequest<IReadOnlyList<PaymentProviderAdminDto>>;

    public class GetAdminPaymentProvidersQueryHandler
        : IRequestHandler<GetAdminPaymentProvidersQuery, IReadOnlyList<PaymentProviderAdminDto>>
    {
        private readonly IPaymentProviderService _paymentProviderService;

        public GetAdminPaymentProvidersQueryHandler(
            IPaymentProviderService paymentProviderService)
        {
            _paymentProviderService = paymentProviderService;
        }

        public Task<IReadOnlyList<PaymentProviderAdminDto>> Handle(
            GetAdminPaymentProvidersQuery request,
            CancellationToken cancellationToken)
        {
            return _paymentProviderService.GetAdminListAsync(cancellationToken);
        }
    }
}
