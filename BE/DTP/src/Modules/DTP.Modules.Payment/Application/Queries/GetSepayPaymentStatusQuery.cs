using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Queries
{
    //public sealed record GetSepayPaymentStatusQuery(Guid OrderId)
    // : IRequest<SepayPaymentStatusDto>;


    //public sealed class GetSepayPaymentStatusQueryHandler
    //: IRequestHandler<GetSepayPaymentStatusQuery, SepayPaymentStatusDto>
    //{
    //    private readonly ISepayPaymentService _sepayPaymentService;

    //    public GetSepayPaymentStatusQueryHandler(ISepayPaymentService sepayPaymentService)
    //    {
    //        _sepayPaymentService = sepayPaymentService;
    //    }

    //    public Task<SepayPaymentStatusDto> Handle(
    //        GetSepayPaymentStatusQuery request,
    //        CancellationToken cancellationToken)
    //    {
    //        return _sepayPaymentService.GetStatusAsync(request.OrderId, cancellationToken);
    //    }
    //}
}
