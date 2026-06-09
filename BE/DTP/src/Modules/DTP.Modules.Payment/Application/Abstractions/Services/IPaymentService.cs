using DTP.Modules.Payment.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface IPaymentService
    {
        Task<CreatePaymentResultDto> CreatePaymentAsync(
            CreatePaymentDto request,
            CancellationToken cancellationToken = default);

        Task<bool> HandleVnptEpayCallbackAsync(
            VnptEpayCallbackDto request,
            CancellationToken cancellationToken = default);

        Task<PaymentTransactionDto?> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}
