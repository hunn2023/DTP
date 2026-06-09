using DTP.Modules.Auth.Application.Abstractions.Repositories;
using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Modules.Auth.Domain.Entities;
using DTP.Shared.Application;


namespace DTP.Modules.Auth.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPendingRegistrationRepository _pendingRegistrationRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IOtpService _otpService;
        private readonly IJwtService _jwtService;
        private readonly IRateLimitService _rateLimitService;
        private readonly IEmailSender _emailSender;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPendingRegistrationRepository pendingRegistrationRepository,
            IPasswordHasher passwordHasher,
            IOtpService otpService,
            IJwtService jwtService,
            IRateLimitService rateLimitService,
            IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _pendingRegistrationRepository = pendingRegistrationRepository;
            _passwordHasher = passwordHasher;
            _otpService = otpService;
            _jwtService = jwtService;
            _rateLimitService = rateLimitService;
            _emailSender = emailSender;
        }

        public async Task<Result> RegisterAsync(
            RegisterRequestDto request,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default)
        {
            var email = request.Email.Trim().ToLower();
            var ip = ipAddress ?? "unknown";

            var ipAllowed = await _rateLimitService.IsAllowedAsync(
                $"auth:register:ip:{ip}",
                5,
                TimeSpan.FromMinutes(10));

            if (!ipAllowed)
                return Result.Failure("Bạn đăng ký quá nhiều lần. Vui lòng thử lại sau.");

            var emailAllowed = await _rateLimitService.IsAllowedAsync(
                $"auth:register:email:{email}",
                3,
                TimeSpan.FromMinutes(10));

            if (!emailAllowed)
                return Result.Failure("Email này gửi yêu cầu quá nhiều lần. Vui lòng thử lại sau.");

            var exists = await _userRepository.ExistsByEmailAsync(email, null, cancellationToken);

            if (exists)
                return Result.Failure("Email đã tồn tại.");

            await _pendingRegistrationRepository.DeleteByEmailAsync(email, cancellationToken);

            var otp = _otpService.GenerateOtp();

            var pending = new PendingRegistration
            {
                Email = email,
                Phone = request.Phone,
                FullName = request.FullName.Trim(),
                PasswordHash = _passwordHasher.Hash(request.Password),
                OtpCodeHash = _otpService.HashOtp(otp),
                OtpExpiredAt = DateTime.UtcNow.AddMinutes(10),
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            await _pendingRegistrationRepository.AddAsync(pending, cancellationToken);
            await _pendingRegistrationRepository.SaveChangesAsync(cancellationToken);

            await _emailSender.SendOtpAsync(email, otp);


            return Result.Success();
        }

        public async Task<Result> VerifyRegisterOtpAsync(
            VerifyOtpRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var email = request.Email.Trim().ToLower();

            var pending = await _pendingRegistrationRepository.GetByEmailAsync(email, cancellationToken);

            if (pending == null)
                return Result.Failure("Không tìm thấy yêu cầu đăng ký.");

            if (pending.OtpExpiredAt < DateTime.UtcNow)
                return Result.Failure("OTP đã hết hạn.");

            if (pending.VerifyFailedCount >= 5)
                return Result.Failure("Bạn nhập sai OTP quá nhiều lần. Vui lòng thử lại sau.");

            var validOtp = _otpService.VerifyOtp(request.OtpCode, pending.OtpCodeHash);

            if (!validOtp)
            {
                pending.VerifyFailedCount++;
                _pendingRegistrationRepository.Update(pending);
                await _pendingRegistrationRepository.SaveChangesAsync(cancellationToken);


                return Result.Failure("OTP không đúng.");

            }

            var exists = await _userRepository.ExistsByEmailAsync(email, null, cancellationToken);

            if (exists)
                return Result.Failure("Email đã tồn tại.");

            var role = await _roleRepository.GetByCodeAsync("CUSTOMER", cancellationToken);

            if (role == null) return Result.Failure("Role CUSTOMER không tồn tại.");
            var user = new User
            {
                Email = pending.Email,
                Phone = pending.Phone,
                FullName = pending.FullName,
                PasswordHash = pending.PasswordHash,
                EmailConfirmed = true,
                IsActive = true
            };

            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });

            await _userRepository.AddAsync(user, cancellationToken);

            _pendingRegistrationRepository.Remove(pending);

            await _userRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(
            LoginRequestDto request,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var email = request.Email.Trim().ToLower();
            var ip = ipAddress ?? "unknown";

            var allowed = await _rateLimitService.IsAllowedAsync(
                $"auth:login:email:{email}",
                10,
                TimeSpan.FromMinutes(15));

            if (!allowed)
                return Result<LoginResponseDto>.Failure("Bạn đăng nhập sai quá nhiều lần. Vui lòng thử lại sau.");

            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null)
                return Result<LoginResponseDto>.Failure("Email hoặc mật khẩu không đúng.");

            if (!user.IsActive)
                return Result<LoginResponseDto>.Failure("Tài khoản đã bị khóa.");

            var validPassword = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!validPassword)
                return Result<LoginResponseDto>.Failure("Email hoặc mật khẩu không đúng.");

            return await GenerateLoginResponseAsync(user, ip, cancellationToken);
        }

        public async Task<Result<LoginResponseDto>> RefreshTokenAsync(
            string refreshToken,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var tokenHash = _jwtService.HashToken(refreshToken);

            var oldToken = await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

            if (oldToken == null || !oldToken.IsActive)
                return Result<LoginResponseDto>.Failure("Refresh token không hợp lệ.");

            var user = await _userRepository.GetByIdWithRolesAsync(
                oldToken.UserId,
                cancellationToken);

            if (user == null || !user.IsActive)
                return Result<LoginResponseDto>.Failure("Tài khoản không hợp lệ.");

            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var newRefreshTokenHash = _jwtService.HashToken(newRefreshToken);

            oldToken.RevokedAt = DateTime.UtcNow;
            oldToken.RevokedByIp = ipAddress;
            oldToken.ReplacedByTokenHash = newRefreshTokenHash;

            _refreshTokenRepository.Update(oldToken);

            var newTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newRefreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                CreatedByIp = ipAddress
            };

            await _refreshTokenRepository.AddAsync(newTokenEntity, cancellationToken);

            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            var roles = user.UserRoles.Select(x => x.Role.Code).ToList();

            var permissions = await _permissionRepository.GetPermissionCodesByUserIdAsync(
                user.Id,
                cancellationToken);

            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.FullName, roles, permissions);


            return Result<LoginResponseDto>.Success(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = MapUserDto(user),
                Permissions = permissions
            });
        }

        public async Task<Result> LogoutAsync(
            string refreshToken,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var tokenHash = _jwtService.HashToken(refreshToken);

            var token = await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

            if (token == null) return Result.Success(); // Token không tồn tại, coi như đã logout

            if (!token.IsRevoked)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;

                _refreshTokenRepository.Update(token);

                await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }

        public async Task<Result<UserDto>> GetProfileAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId, cancellationToken);

            if (user == null)
                return Result<UserDto>.Failure("Không tìm thấy user.");

            return Result<UserDto>.Success(MapUserDto(user));

        }

        private async Task<Result<LoginResponseDto>> GenerateLoginResponseAsync(
            User user,
            string? ipAddress,
            CancellationToken cancellationToken)
        {
            var roles = user.UserRoles.Select(x => x.Role.Code).ToList();

            var permissions = await _permissionRepository.GetPermissionCodesByUserIdAsync(
                user.Id,
                cancellationToken);

            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.FullName, roles, permissions);

            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenHash = _jwtService.HashToken(refreshToken);

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                CreatedByIp = ipAddress
            };

            user.LastLoginAt = DateTime.UtcNow;

            await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

            _userRepository.Update(user);

            await _userRepository.SaveChangesAsync(cancellationToken);


            return Result<LoginResponseDto>.Success(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = MapUserDto(user),
                Permissions = permissions
            });
        }

        private static UserDto MapUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed
            };
        }
    }
}
