using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Payment.Application.Commands.Sepay
{
    public sealed record CreateSepayQrCommand(Guid OrderId, string IpAddress)
    : IRequest<Result<PaymentQrResponseDto>>;

    public sealed class CreateSepayQrCommandHandler
    : IRequestHandler<CreateSepayQrCommand, Result<PaymentQrResponseDto>>
    {
        private readonly ISepayPaymentService _sepayPaymentService;

        public CreateSepayQrCommandHandler(ISepayPaymentService sepayPaymentService)
        {
            _sepayPaymentService = sepayPaymentService;
        }

        public Task<Result<PaymentQrResponseDto>> Handle(
            CreateSepayQrCommand request,
            CancellationToken cancellationToken)
        {
            return _sepayPaymentService.CreateQrAsync(request.OrderId, request.IpAddress, cancellationToken);
        }
    }
}
