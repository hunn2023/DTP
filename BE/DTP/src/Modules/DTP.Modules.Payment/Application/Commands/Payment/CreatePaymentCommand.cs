using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Commands.Payment
{
    public class CreatePaymentCommand : IRequest<CreatePaymentResultDto>
    {
        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public decimal Amount { get; set; }

        public string CurrencyCode { get; set; } = "VND";

        public string CustomerEmail { get; set; } = default!;

        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }

        public string? ReturnUrl { get; set; }

        public string? CallbackUrl { get; set; }
    }

    public class CreatePaymentCommandHandler
       : IRequestHandler<CreatePaymentCommand, CreatePaymentResultDto>
    {
        private readonly IPaymentService _paymentService;

        public CreatePaymentCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<CreatePaymentResultDto> Handle(
            CreatePaymentCommand request,
            CancellationToken cancellationToken)
        {
            return await _paymentService.CreatePaymentAsync(new CreatePaymentDto
            {
                OrderId = request.OrderId,
                OrderCode = request.OrderCode,
                Amount = request.Amount,
                CurrencyCode = request.CurrencyCode,
                CustomerEmail = request.CustomerEmail,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                ReturnUrl = request.ReturnUrl,
                CallbackUrl = request.CallbackUrl
            }, cancellationToken);
        }
    }
}
