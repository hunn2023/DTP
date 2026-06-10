using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Application.Abstractions.Services
{
    public interface IAuthRateLimitService
    {
        Task<bool> IsLoginBlockedAsync(
            string email,
            string ipAddress,
            CancellationToken cancellationToken = default);

        Task RegisterLoginFailedAsync(
            string email,
            string ipAddress,
            CancellationToken cancellationToken = default);

        Task RegisterLoginSuccessAsync(
            string email,
            string ipAddress,
            CancellationToken cancellationToken = default);

        Task<bool> IsOtpBlockedAsync(
            string target,
            string ipAddress,
            CancellationToken cancellationToken = default);

        Task RegisterOtpSentAsync(
            string target,
            string ipAddress,
            CancellationToken cancellationToken = default);

        Task<bool> IsRegisterBlockedAsync(
            string email,
            string ipAddress,
            CancellationToken cancellationToken = default);

        Task RegisterRegisterAttemptAsync(
            string email,
            string ipAddress,
            CancellationToken cancellationToken = default);

        Task RegisterOtpVerifyFailedAsync(
            string target,
            string ipAddress,
            CancellationToken cancellationToken = default);

        Task RegisterOtpVerifySuccessAsync(
            string target,
            string ipAddress,
            CancellationToken cancellationToken = default);
    }
}
