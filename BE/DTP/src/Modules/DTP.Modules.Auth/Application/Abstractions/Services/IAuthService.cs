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
    }
}
