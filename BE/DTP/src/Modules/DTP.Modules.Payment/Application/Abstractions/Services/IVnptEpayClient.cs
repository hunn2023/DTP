using DTP.Modules.Payment.Application.DTOs;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface IVnptEpayClient
    {
        Task<VnptEpayRegisterVaResponse> RegisterVaAsync(
            VnptEpayRegisterVaRequest request,
            CancellationToken cancellationToken = default);

        bool Verify(
            string rawString,
            string signatureHex,
            string epayPublicKeyPem);

        bool VerifyDepositNotificationSignature(Application.DTOs.VnptEpayCallbackDto callback);
    }
}
