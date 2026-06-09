using DTP.Modules.Payment.Application.DTOs;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface IVnptEpayClient
    {

        Task<VnptEpayCreateQrResponse> CreateQrAsync(
            VnptEpayCreateQrRequest request,
            CancellationToken cancellationToken = default);

        bool VerifyCallbackSignature(VnptEpayCallbackDto request);
    }
}
