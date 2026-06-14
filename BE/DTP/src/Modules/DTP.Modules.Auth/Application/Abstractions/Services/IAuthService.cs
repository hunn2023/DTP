using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;


namespace DTP.Modules.Auth.Application.Abstractions.Services
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterRequestDto request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);

        Task<Result> VerifyRegisterOtpAsync(VerifyOtpRequestDto request, CancellationToken cancellationToken = default);

        Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);

        Task<Result<LoginResponseDto>> RefreshTokenAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default);

        Task<Result> LogoutAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default);

        Task<Result<UserDto>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);


        Task<Result> ResendRegisterOtpAsync(
                ResendRegisterOtpRequestDto request,
                string? ipAddress,
                string? userAgent,
                CancellationToken cancellationToken = default);


        Task<Result> ForgotPasswordAsync(
            ForgotPasswordRequestDto request,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default);

        Task<Result> ResetPasswordAsync(
            ResetPasswordRequestDto request,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default);


        Task<Result> ChangePasswordAsync(
            Guid userId,
            ChangePasswordRequestDto request,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default);
    }
}
