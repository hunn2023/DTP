using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Domain.Enums;
using DTP.Modules.Auth.Application.Abstractions.Repositories;
using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Modules.Auth.Domain.Entities;
using DTP.Shared.Application;
using DTP.Shared.Application.Emails;
using MimeKit;


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
        private readonly IAuditLogWriter _auditLogWriter;
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
            IEmailSender emailSender,
            IAuditLogWriter auditLogWriter)
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
            _auditLogWriter = auditLogWriter;
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
            {
                await WriteAuditSafeAsync(
                    action: "Register Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "PendingRegistration",
                    description: "Register failed because IP rate limit exceeded.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        Reason = "IP rate limit exceeded"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Bạn đăng ký quá nhiều lần. Vui lòng thử lại sau.");
            }

            var emailAllowed = await _rateLimitService.IsAllowedAsync(
                $"auth:register:email:{email}",
                3,
                TimeSpan.FromMinutes(10));

            if (!emailAllowed)
            {
                await WriteAuditSafeAsync(
                    action: "Register Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "PendingRegistration",
                    description: "Register failed because email rate limit exceeded.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        Reason = "Email rate limit exceeded"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Email này gửi yêu cầu quá nhiều lần. Vui lòng thử lại sau.");
            }

            var exists = await _userRepository.ExistsByEmailAsync(
                email,
                null,
                cancellationToken);

            if (exists)
            {
                await WriteAuditSafeAsync(
                    action: "Register Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    description: "Register failed because email already exists.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        Reason = "Email already exists"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Email đã tồn tại.");
            }

            await _pendingRegistrationRepository.DeleteByEmailAsync(
                email,
                cancellationToken);

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

            await _pendingRegistrationRepository.AddAsync(
                pending,
                cancellationToken);

            await _pendingRegistrationRepository.SaveChangesAsync(
                cancellationToken);


            var emailBody = new BodyBuilder
            {
                HtmlBody = $@"
                <div style='font-family:Arial,sans-serif'>
                    <h2>Xác thực tài khoản DTP</h2>
                    <p>Mã OTP của bạn là:</p>
                    <h1 style='letter-spacing:4px'>{otp}</h1>
                    <p>Mã này có hiệu lực trong 10 phút.</p>
                    <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.</p>
                </div>"
            }.ToString();


            await _emailSender.SendAsync(email, "Mã xác thực đăng ký DTP", emailBody);

            await WriteAuditSafeAsync(
                action: "Register Requested",
                actionType: AuditActionType.Create,
                status: AuditStatus.Success,
                entityName: "PendingRegistration",
                entityId: pending.Id,
                description: "User requested registration OTP.",
                newValues: new
                {
                    pending.Id,
                    pending.Email,
                    pending.Phone,
                    pending.FullName,
                    pending.OtpExpiredAt,
                    IpAddress = ip,
                    UserAgent = userAgent
                },
                cancellationToken: cancellationToken);

            return Result.Success();
        }

        public async Task<Result> VerifyRegisterOtpAsync(
    VerifyOtpRequestDto request,
    CancellationToken cancellationToken = default)
        {
            var email = request.Email.Trim().ToLower();

            var pending = await _pendingRegistrationRepository.GetByEmailAsync(
                email,
                cancellationToken);

            if (pending == null)
            {
                await WriteAuditSafeAsync(
                    action: "Verify Register OTP Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "PendingRegistration",
                    description: "Verify register OTP failed because pending registration was not found.",
                    newValues: new
                    {
                        Email = email,
                        Reason = "Pending registration not found"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Không tìm thấy yêu cầu đăng ký.");
            }

            if (pending.OtpExpiredAt < DateTime.UtcNow)
            {
                await WriteAuditSafeAsync(
                    action: "Verify Register OTP Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "PendingRegistration",
                    entityId: pending.Id,
                    description: "Verify register OTP failed because OTP expired.",
                    newValues: new
                    {
                        pending.Id,
                        pending.Email,
                        pending.OtpExpiredAt,
                        Reason = "OTP expired"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("OTP đã hết hạn.");
            }

            if (pending.VerifyFailedCount >= 5)
            {
                await WriteAuditSafeAsync(
                    action: "Verify Register OTP Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "PendingRegistration",
                    entityId: pending.Id,
                    description: "Verify register OTP failed because failed count exceeded.",
                    newValues: new
                    {
                        pending.Id,
                        pending.Email,
                        pending.VerifyFailedCount,
                        Reason = "Verify failed count exceeded"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Bạn nhập sai OTP quá nhiều lần. Vui lòng thử lại sau.");
            }

            var validOtp = _otpService.VerifyOtp(
                request.OtpCode,
                pending.OtpCodeHash);

            if (!validOtp)
            {
                pending.VerifyFailedCount++;

                _pendingRegistrationRepository.Update(pending);

                await _pendingRegistrationRepository.SaveChangesAsync(
                    cancellationToken);

                await WriteAuditSafeAsync(
                    action: "Verify Register OTP Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "PendingRegistration",
                    entityId: pending.Id,
                    description: "Verify register OTP failed because OTP is invalid.",
                    newValues: new
                    {
                        pending.Id,
                        pending.Email,
                        pending.VerifyFailedCount,
                        Reason = "Invalid OTP"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("OTP không đúng.");
            }

            var exists = await _userRepository.ExistsByEmailAsync(
                email,
                null,
                cancellationToken);

            if (exists)
            {
                await WriteAuditSafeAsync(
                    action: "Verify Register OTP Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    description: "Verify register OTP failed because email already exists.",
                    newValues: new
                    {
                        Email = email,
                        Reason = "Email already exists"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Email đã tồn tại.");
            }

            var role = await _roleRepository.GetByCodeAsync(
                "CUSTOMER",
                cancellationToken);

            if (role == null)
            {
                await WriteAuditSafeAsync(
                    action: "Verify Register OTP Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "Role",
                    description: "Verify register OTP failed because CUSTOMER role does not exist.",
                    newValues: new
                    {
                        Email = email,
                        RoleCode = "CUSTOMER",
                        Reason = "Role CUSTOMER not found"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Role CUSTOMER không tồn tại.");
            }

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

            await _userRepository.AddAsync(
                user,
                cancellationToken);

            _pendingRegistrationRepository.Remove(pending);

            await _userRepository.SaveChangesAsync(
                cancellationToken);

            await WriteAuditSafeAsync(
                action: "Register Verified",
                actionType: AuditActionType.Create,
                status: AuditStatus.Success,
                entityName: "User",
                entityId: user.Id,
                description: "User registered successfully after OTP verification.",
                newValues: new
                {
                    user.Id,
                    user.Email,
                    user.Phone,
                    user.FullName,
                    user.EmailConfirmed,
                    user.IsActive,
                    Role = role.Code
                },
                cancellationToken: cancellationToken);

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
            {
                await WriteAuditSafeAsync(
                    action: "Login Failed",
                    actionType: AuditActionType.Login,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    description: "Login failed because email rate limit exceeded.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        Reason = "Login rate limit exceeded"
                    },
                    cancellationToken: cancellationToken);

                return Result<LoginResponseDto>.Failure("Bạn đăng nhập sai quá nhiều lần. Vui lòng thử lại sau.");
            }

            var user = await _userRepository.GetByEmailAsync(
                email,
                cancellationToken);

            if (user == null)
            {
                await WriteAuditSafeAsync(
                    action: "Login Failed",
                    actionType: AuditActionType.Login,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    description: "Login failed because user was not found.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        Reason = "User not found"
                    },
                    cancellationToken: cancellationToken);

                return Result<LoginResponseDto>.Failure("Email hoặc mật khẩu không đúng.");
            }

            if (!user.IsActive)
            {
                await WriteAuditSafeAsync(
                    action: "Login Failed",
                    actionType: AuditActionType.Login,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: user.Id,
                    description: "Login failed because user is inactive.",
                    newValues: new
                    {
                        user.Id,
                        user.Email,
                        user.FullName,
                        user.IsActive,
                        IpAddress = ip,
                        Reason = "User inactive"
                    },
                    cancellationToken: cancellationToken);

                return Result<LoginResponseDto>.Failure("Tài khoản đã bị khóa.");
            }

            var validPassword = _passwordHasher.Verify(
                request.Password,
                user.PasswordHash);

            if (!validPassword)
            {
                await WriteAuditSafeAsync(
                    action: "Login Failed",
                    actionType: AuditActionType.Login,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: user.Id,
                    description: "Login failed because password is invalid.",
                    newValues: new
                    {
                        user.Id,
                        user.Email,
                        user.FullName,
                        IpAddress = ip,
                        Reason = "Invalid password"
                    },
                    cancellationToken: cancellationToken);

                return Result<LoginResponseDto>.Failure("Email hoặc mật khẩu không đúng.");
            }

            var result = await GenerateLoginResponseAsync(
                user,
                ip,
                cancellationToken);

            if (!result.IsSuccess)
            {
                await WriteAuditSafeAsync(
                    action: "Login Failed",
                    actionType: AuditActionType.Login,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: user.Id,
                    description: "Login failed because token generation failed.",
                    newValues: new
                    {
                        user.Id,
                        user.Email,
                        IpAddress = ip,
                        Reason = "Generate login response failed"
                    },
                    cancellationToken: cancellationToken);

                return result;
            }

            await WriteAuditSafeAsync(
                action: "Login Success",
                actionType: AuditActionType.Login,
                status: AuditStatus.Success,
                entityName: "User",
                entityId: user.Id,
                description: "User logged in successfully.",
                newValues: new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    LoginAt = DateTime.UtcNow,
                    IpAddress = ip
                },
                cancellationToken: cancellationToken);

            return result;
        }


        public async Task<Result<LoginResponseDto>> RefreshTokenAsync(
    string refreshToken,
    string? ipAddress,
    CancellationToken cancellationToken = default)
        {
            var ip = ipAddress ?? "unknown";

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                await WriteAuditSafeAsync(
                    action: "Refresh Token Failed",
                    actionType: AuditActionType.Login,
                    status: AuditStatus.Failed,
                    entityName: "RefreshToken",
                    description: "Refresh token failed because refresh token is empty.",
                    newValues: new
                    {
                        IpAddress = ip,
                        Reason = "Refresh token empty"
                    },
                    cancellationToken: cancellationToken);

                return Result<LoginResponseDto>.Failure("Refresh token không hợp lệ.");
            }

            var tokenHash = _jwtService.HashToken(refreshToken);

            var oldToken = await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

            if (oldToken == null || !oldToken.IsActive)
            {
                await WriteAuditSafeAsync(
                    action: "Refresh Token Failed",
                    actionType: AuditActionType.Login,
                    status: AuditStatus.Failed,
                    entityName: "RefreshToken",
                    description: "Refresh token failed because token is invalid or inactive.",
                    newValues: new
                    {
                        IpAddress = ip,
                        Reason = "Invalid or inactive refresh token"
                    },
                    cancellationToken: cancellationToken);

                return Result<LoginResponseDto>.Failure("Refresh token không hợp lệ.");
            }

            var user = await _userRepository.GetByIdWithRolesAsync(
                oldToken.UserId,
                cancellationToken);

            if (user == null || !user.IsActive)
            {
                await WriteAuditSafeAsync(
                    action: "Refresh Token Failed",
                    actionType: AuditActionType.Login,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: oldToken.UserId,
                    description: "Refresh token failed because user is invalid or inactive.",
                    newValues: new
                    {
                        UserId = oldToken.UserId,
                        IpAddress = ip,
                        Reason = "User invalid or inactive"
                    },
                    cancellationToken: cancellationToken);

                return Result<LoginResponseDto>.Failure("Tài khoản không hợp lệ.");
            }

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

            await _refreshTokenRepository.AddAsync(
                newTokenEntity,
                cancellationToken);

            await _refreshTokenRepository.SaveChangesAsync(
                cancellationToken);

            var roles = user.UserRoles
                .Select(x => x.Role.Code)
                .ToList();

            var permissions = await _permissionRepository.GetPermissionCodesByUserIdAsync(
                user.Id,
                cancellationToken);

            var accessToken = _jwtService.GenerateAccessToken(
                user.Id,
                user.Email,
                user.FullName,
                roles,
                permissions);

            await WriteAuditSafeAsync(
                action: "Refresh Token Success",
                actionType: AuditActionType.Login,
                status: AuditStatus.Success,
                entityName: "RefreshToken",
                entityId: newTokenEntity.Id,
                description: "Refresh token renewed successfully.",
                newValues: new
                {
                    UserId = user.Id,
                    user.Email,
                    OldRefreshTokenId = oldToken.Id,
                    NewRefreshTokenId = newTokenEntity.Id,
                    newTokenEntity.ExpiresAt,
                    IpAddress = ip
                },
                cancellationToken: cancellationToken);

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
            var ip = ipAddress ?? "unknown";

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                await WriteAuditSafeAsync(
                    action: "Logout Success",
                    actionType: AuditActionType.Logout,
                    status: AuditStatus.Success,
                    entityName: "RefreshToken",
                    description: "Logout requested with empty refresh token.",
                    newValues: new
                    {
                        IpAddress = ip,
                        Reason = "Empty refresh token"
                    },
                    cancellationToken: cancellationToken);

                return Result.Success();
            }

            var tokenHash = _jwtService.HashToken(refreshToken);

            var token = await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

            if (token == null)
            {
                await WriteAuditSafeAsync(
                    action: "Logout Success",
                    actionType: AuditActionType.Logout,
                    status: AuditStatus.Success,
                    entityName: "RefreshToken",
                    description: "Logout requested but refresh token was not found.",
                    newValues: new
                    {
                        IpAddress = ip,
                        Reason = "Refresh token not found"
                    },
                    cancellationToken: cancellationToken);

                return Result.Success();
            }

            if (!token.IsRevoked)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;

                _refreshTokenRepository.Update(token);

                await _refreshTokenRepository.SaveChangesAsync(
                    cancellationToken);
            }

            await WriteAuditSafeAsync(
                action: "Logout Success",
                actionType: AuditActionType.Logout,
                status: AuditStatus.Success,
                entityName: "RefreshToken",
                entityId: token.Id,
                description: "User logged out successfully.",
                newValues: new
                {
                    token.Id,
                    token.UserId,
                    token.RevokedAt,
                    token.RevokedByIp,
                    IpAddress = ip
                },
                cancellationToken: cancellationToken);

            return Result.Success();
        }

        public async Task<Result<UserDto>> GetProfileAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(
                userId,
                cancellationToken);

            if (user == null)
            {
                await WriteAuditSafeAsync(
                    action: "View Profile Failed",
                    actionType: AuditActionType.View,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: userId,
                    description: "View profile failed because user was not found.",
                    newValues: new
                    {
                        UserId = userId,
                        Reason = "User not found"
                    },
                    cancellationToken: cancellationToken);

                return Result<UserDto>.Failure("Không tìm thấy user.");
            }

            await WriteAuditSafeAsync(
                action: "View Profile",
                actionType: AuditActionType.View,
                status: AuditStatus.Success,
                entityName: "User",
                entityId: user.Id,
                description: "User viewed profile.",
                newValues: new
                {
                    user.Id,
                    user.Email,
                    user.FullName
                },
                cancellationToken: cancellationToken);

            return Result<UserDto>.Success(MapUserDto(user));
        }

        private async Task<Result<LoginResponseDto>> GenerateLoginResponseAsync(
       User user,
       string? ipAddress,
       CancellationToken cancellationToken)
        {
            var roles = user.UserRoles
                .Select(x => x.Role.Code)
                .ToList();

            var permissions = await _permissionRepository.GetPermissionCodesByUserIdAsync(
                user.Id,
                cancellationToken);

            var accessToken = _jwtService.GenerateAccessToken(
                user.Id,
                user.Email,
                user.FullName,
                roles,
                permissions);

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

            await _refreshTokenRepository.AddAsync(
                refreshTokenEntity,
                cancellationToken);

            _userRepository.Update(user);

            await _userRepository.SaveChangesAsync(
                cancellationToken);

            return Result<LoginResponseDto>.Success(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = MapUserDto(user),
                Permissions = permissions
            });
        }

        private async Task WriteAuditSafeAsync(
            string action,
            AuditActionType actionType,
            AuditStatus status,
            string? entityName = null,
            Guid? entityId = null,
            string? description = null,
            object? oldValues = null,
            object? newValues = null,
            string? errorMessage = null,
            CancellationToken cancellationToken = default)
        {
            await _auditLogWriter.WriteAsync(
                module: "Auth",
                action: action,
                actionType: actionType,
                status: status,
                entityName: entityName,
                entityId: entityId,
                description: description,
                oldValues: oldValues,
                newValues: newValues,
                errorMessage: errorMessage,
                cancellationToken: cancellationToken);
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
