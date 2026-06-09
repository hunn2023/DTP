using DTP.Modules.Payment.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface IVnptEpayClient
    {
        Task<VnptEpayCreateQrResult> CreateQrPaymentAsync(
            VnptEpayCreateQrRequest request,
            CancellationToken cancellationToken = default);

        bool VerifyCallbackSignature(VnptEpayCallbackDto request);
    }
}
