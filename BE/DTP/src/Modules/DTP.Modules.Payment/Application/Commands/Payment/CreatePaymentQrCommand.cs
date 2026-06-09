using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Payment.Application.Commands.Payment
{
    public class CreatePaymentQrCommand : IRequest<Result<PaymentQrResponseDto>>
    {
        public Guid OrderId { get; set; }

        public string IpAddress { get; set; } = default!;
    }

    public class CreatePaymentQrCommandHandler
    : IRequestHandler<CreatePaymentQrCommand, Result<PaymentQrResponseDto>>
    {
        private readonly IPaymentService _paymentService;

        public CreatePaymentQrCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public Task<Result<PaymentQrResponseDto>> Handle(
            CreatePaymentQrCommand request,
            CancellationToken cancellationToken)
        {
            return _paymentService.CreateQrAsync(
                request.OrderId,
                request.IpAddress,
                cancellationToken);
        }
    }
}
