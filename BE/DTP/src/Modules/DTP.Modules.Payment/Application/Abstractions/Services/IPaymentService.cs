using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Shared.Application;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface IPaymentService
    {
        Task<Result<PaymentTransactionDto>> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task<Result<PaymentTransactionDto>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
