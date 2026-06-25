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
        private readonly IEmailSender _emailSender;
        private readonly IAuditLogWriter _auditLogWriter;
        private readonly IAuthRateLimitService _authRateLimitService;
        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPendingRegistrationRepository pendingRegistrationRepository,
            IPasswordHasher passwordHasher,
            IOtpService otpService,
            IJwtService jwtService,
            IAuthRateLimitService authRateLimitService,
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
            _authRateLimitService = authRateLimitService;
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

            var registerBlocked = await _authRateLimitService.IsRegisterBlockedAsync(
                                email,
                                ip,
                                cancellationToken);

            if (registerBlocked)
            {
                await WriteAuditSafeAsync(
                    action: "Register Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "PendingRegistration",
                    description: "Register failed because register rate limit exceeded.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        Reason = "Register rate limit exceeded"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Bạn đăng ký quá nhiều lần. Vui lòng thử lại sau.");
            }

            await _authRateLimitService.RegisterRegisterAttemptAsync(
                    email,
                    ip,
                    cancellationToken);



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


            var emailBody = BuildRegisterOtpEmailBody(otp);


            await _emailSender.SendAsync(email, $"{otp} - Mã xác thực đăng ký ezsim của bạn", emailBody);

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
            var ipAddress = request?.IpAddress?.Trim();
            var userAgent = request?.UserAgent?.Trim();
            ipAddress = string.IsNullOrWhiteSpace(ipAddress)
                ? "unknown"
                : ipAddress.Trim();

            userAgent = string.IsNullOrWhiteSpace(userAgent)
                ? null
                : userAgent.Trim();

            var isOtpBlocked = await _authRateLimitService.IsOtpBlockedAsync(
                email,
                ipAddress,
                cancellationToken);

            if (isOtpBlocked)
            {
                await WriteAuditSafeAsync(
                    action: "Verify Register OTP Blocked",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "PendingRegistration",
                    description: "Verify register OTP was blocked by rate limit.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        Reason = "OTP verify rate limit exceeded",
                        BlockedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Bạn nhập OTP quá nhiều lần. Vui lòng thử lại sau 15 phút.");
            }

            var pending = await _pendingRegistrationRepository.GetByEmailAsync(
                email,
                cancellationToken);

            if (pending == null)
            {
                await _authRateLimitService.RegisterOtpVerifyFailedAsync(
                    email,
                    ipAddress,
                    cancellationToken);

                await WriteAuditSafeAsync(
                    action: "Verify Register OTP Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "PendingRegistration",
                    description: "Verify register OTP failed because pending registration was not found.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        Reason = "Pending registration not found",
                        FailedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Không tìm thấy yêu cầu đăng ký.");
            }

            if (pending.OtpExpiredAt < DateTime.UtcNow)
            {
                await _authRateLimitService.RegisterOtpVerifyFailedAsync(
                    email,
                    ipAddress,
                    cancellationToken);

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
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        Reason = "OTP expired",
                        FailedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("OTP đã hết hạn.");
            }

            if (pending.VerifyFailedCount >= 5)
            {
                await _authRateLimitService.RegisterOtpVerifyFailedAsync(
                    email,
                    ipAddress,
                    cancellationToken);

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
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        Reason = "Verify failed count exceeded",
                        FailedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Bạn nhập sai OTP quá nhiều lần. Vui lòng thử lại sau.");
            }

            var validOtp = _otpService.VerifyOtp(
                request?.OtpCode ?? "",
                pending.OtpCodeHash);

            if (!validOtp)
            {
                pending.VerifyFailedCount++;

                _pendingRegistrationRepository.Update(pending);

                await _pendingRegistrationRepository.SaveChangesAsync(
                    cancellationToken);

                await _authRateLimitService.RegisterOtpVerifyFailedAsync(
                    email,
                    ipAddress,
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
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        Reason = "Invalid OTP",
                        FailedAt = DateTime.UtcNow
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
                await _authRateLimitService.RegisterOtpVerifyFailedAsync(
                    email,
                    ipAddress,
                    cancellationToken);

                await WriteAuditSafeAsync(
                    action: "Verify Register OTP Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    description: "Verify register OTP failed because email already exists.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        Reason = "Email already exists",
                        FailedAt = DateTime.UtcNow
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
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        Reason = "Role CUSTOMER not found",
                        FailedAt = DateTime.UtcNow
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

            await _authRateLimitService.RegisterOtpVerifySuccessAsync(
                email,
                ipAddress,
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
                    Role = role.Code,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    VerifiedAt = DateTime.UtcNow
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

            var loginBlocked = await _authRateLimitService.IsLoginBlockedAsync(
                    email,
                    ip,
                    cancellationToken);

            if (loginBlocked)
            {
                await WriteAuditSafeAsync(
                    action: "Login Failed",
                    actionType: AuditActionType.Login,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    description: "Login failed because login rate limit exceeded.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        Reason = "Login blocked by email or IP"
                    },
                    cancellationToken: cancellationToken);

                return Result<LoginResponseDto>.Failure(
                    "Bạn đăng nhập sai quá nhiều lần. Vui lòng thử lại sau.");
            }

            var user = await _userRepository.GetByEmailAsync(
                email,
                cancellationToken);

            if (user == null)
            {
                await _authRateLimitService.RegisterLoginFailedAsync(
                          email,
                          ip,
                          cancellationToken);

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
                await _authRateLimitService.RegisterLoginFailedAsync(
                 email,
                 ip,
                 cancellationToken);

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


            await _authRateLimitService.RegisterLoginSuccessAsync(
                    email,
                    ip,
                    cancellationToken);

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


        public async Task<Result> ResendRegisterOtpAsync(
            ResendRegisterOtpRequestDto request,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                return Result.Failure("Dữ liệu gửi lại OTP không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result.Failure("Vui lòng nhập email.");

            var email = request.Email.Trim().ToLowerInvariant();
            var ip = string.IsNullOrWhiteSpace(ipAddress) ? "unknown" : ipAddress.Trim();

            var otpBlocked = await _authRateLimitService.IsOtpBlockedAsync(
                email,
                ip,
                cancellationToken);

            if (otpBlocked)
            {
                await WriteAuditSafeAsync(
                    action: "Resend Register OTP Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "PendingRegistration",
                    description: "Resend register OTP failed because OTP rate limit exceeded.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "OTP rate limit exceeded"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Bạn yêu cầu gửi OTP quá nhiều lần. Vui lòng thử lại sau.");
            }

            var existsUser = await _userRepository.ExistsByEmailAsync(
                email,
                null,
                cancellationToken);

            if (existsUser)
            {
                await WriteAuditSafeAsync(
                    action: "Resend Register OTP Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    description: "Resend register OTP failed because email already exists.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "Email already exists"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Email đã tồn tại.");
            }

            var pending = await _pendingRegistrationRepository.GetByEmailAsync(
                email,
                cancellationToken);

            if (pending == null)
            {
                await WriteAuditSafeAsync(
                    action: "Resend Register OTP Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "PendingRegistration",
                    description: "Resend register OTP failed because pending registration was not found.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "Pending registration not found"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Không tìm thấy yêu cầu đăng ký. Vui lòng đăng ký lại.");
            }

            var otp = _otpService.GenerateOtp();

            pending.OtpCodeHash = _otpService.HashOtp(otp);
            pending.OtpExpiredAt = DateTime.UtcNow.AddMinutes(10);
            pending.VerifyFailedCount = 0;
            pending.IpAddress = ip;
            pending.UserAgent = userAgent;

            _pendingRegistrationRepository.Update(pending);

            await _pendingRegistrationRepository.SaveChangesAsync(
                cancellationToken);

            var emailBody = BuildRegisterOtpEmailBody(otp);

            await _emailSender.SendAsync(
                email,
                "Mã xác thực đăng ký DTP",
                emailBody);

            await _authRateLimitService.RegisterOtpSentAsync(
                email,
                ip,
                cancellationToken);

            await WriteAuditSafeAsync(
                action: "Resend Register OTP Success",
                actionType: AuditActionType.Create,
                status: AuditStatus.Success,
                entityName: "PendingRegistration",
                entityId: pending.Id,
                description: "Register OTP was resent successfully.",
                newValues: new
                {
                    pending.Id,
                    pending.Email,
                    pending.Phone,
                    pending.FullName,
                    pending.OtpExpiredAt,
                    pending.VerifyFailedCount,
                    IpAddress = ip,
                    UserAgent = userAgent
                },
                cancellationToken: cancellationToken);

            return Result.Success();
        }



        public async Task<Result> ForgotPasswordAsync(
            ForgotPasswordRequestDto request,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                return Result.Failure("Dữ liệu quên mật khẩu không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result.Failure("Vui lòng nhập email.");

            var email = request.Email.Trim().ToLowerInvariant();
            var ip = string.IsNullOrWhiteSpace(ipAddress) ? "unknown" : ipAddress.Trim();

            var otpBlocked = await _authRateLimitService.IsOtpBlockedAsync(
                email,
                ip,
                cancellationToken);

            if (otpBlocked)
            {
                await WriteAuditSafeAsync(
                    action: "Forgot Password Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    description: "Forgot password failed because OTP rate limit exceeded.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "OTP rate limit exceeded"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Bạn yêu cầu gửi OTP quá nhiều lần. Vui lòng thử lại sau.");
            }

            var user = await _userRepository.GetByEmailAsync(
                email,
                cancellationToken);

            // Không nên báo email không tồn tại để tránh lộ thông tin tài khoản
            if (user == null || !user.IsActive)
            {
                await WriteAuditSafeAsync(
                    action: "Forgot Password Requested",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Success,
                    entityName: "User",
                    description: "Forgot password requested but user was not found or inactive.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = user == null ? "User not found" : "User inactive"
                    },
                    cancellationToken: cancellationToken);

                return Result.Success();
            }

            var otp = _otpService.GenerateOtp();

            user.PasswordResetOtpHash = _otpService.HashOtp(otp);
            user.PasswordResetOtpExpiredAt = DateTime.UtcNow.AddMinutes(10);
            user.PasswordResetVerifyFailedCount = 0;

            _userRepository.Update(user);

            await _userRepository.SaveChangesAsync(cancellationToken);

            var emailBody = BuildForgotPasswordOtpEmailBody(otp);

            await _emailSender.SendAsync(
                email,
                $"{otp} - Mã đặt lại mật khẩu ezsim của bạn",
                emailBody);

            await _authRateLimitService.RegisterOtpSentAsync(
                email,
                ip,
                cancellationToken);

            await WriteAuditSafeAsync(
                action: "Forgot Password OTP Sent",
                actionType: AuditActionType.Create,
                status: AuditStatus.Success,
                entityName: "User",
                entityId: user.Id,
                description: "Forgot password OTP was sent successfully.",
                newValues: new
                {
                    user.Id,
                    user.Email,
                    user.PasswordResetOtpExpiredAt,
                    IpAddress = ip,
                    UserAgent = userAgent
                },
                cancellationToken: cancellationToken);

            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(
                ResetPasswordRequestDto request,
                string? ipAddress,
                string? userAgent,
                CancellationToken cancellationToken = default)
        {
            if (request == null)
                return Result.Failure("Dữ liệu đặt lại mật khẩu không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result.Failure("Vui lòng nhập email.");

            if (string.IsNullOrWhiteSpace(request.OtpCode))
                return Result.Failure("Vui lòng nhập mã OTP.");

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return Result.Failure("Vui lòng nhập mật khẩu mới.");

            if (request.NewPassword.Length < 6)
                return Result.Failure("Mật khẩu mới phải có ít nhất 6 ký tự.");

            var email = request.Email.Trim().ToLowerInvariant();
            var ip = string.IsNullOrWhiteSpace(ipAddress) ? "unknown" : ipAddress.Trim();

            var otpBlocked = await _authRateLimitService.IsOtpBlockedAsync(
                email,
                ip,
                cancellationToken);

            if (otpBlocked)
            {
                await WriteAuditSafeAsync(
                    action: "Reset Password Blocked",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    description: "Reset password was blocked by rate limit.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "OTP verify rate limit exceeded"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Bạn nhập OTP quá nhiều lần. Vui lòng thử lại sau.");
            }

            var user = await _userRepository.GetByEmailAsync(
                email,
                cancellationToken);

            if (user == null)
            {
                await _authRateLimitService.RegisterOtpVerifyFailedAsync(
                    email,
                    ip,
                    cancellationToken);

                await WriteAuditSafeAsync(
                    action: "Reset Password Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    description: "Reset password failed because user was not found.",
                    newValues: new
                    {
                        Email = email,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "User not found"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("OTP không đúng hoặc đã hết hạn.");
            }

            if (!user.IsActive)
            {
                await WriteAuditSafeAsync(
                    action: "Reset Password Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: user.Id,
                    description: "Reset password failed because user is inactive.",
                    newValues: new
                    {
                        user.Id,
                        user.Email,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "User inactive"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Tài khoản đã bị khóa.");
            }

            if (string.IsNullOrWhiteSpace(user.PasswordResetOtpHash) ||
                user.PasswordResetOtpExpiredAt == null)
            {
                await _authRateLimitService.RegisterOtpVerifyFailedAsync(
                    email,
                    ip,
                    cancellationToken);

                return Result.Failure("OTP không đúng hoặc đã hết hạn.");
            }

            if (user.PasswordResetOtpExpiredAt < DateTime.UtcNow)
            {
                await _authRateLimitService.RegisterOtpVerifyFailedAsync(
                    email,
                    ip,
                    cancellationToken);

                await WriteAuditSafeAsync(
                    action: "Reset Password Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: user.Id,
                    description: "Reset password failed because OTP expired.",
                    newValues: new
                    {
                        user.Id,
                        user.Email,
                        user.PasswordResetOtpExpiredAt,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "OTP expired"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("OTP đã hết hạn.");
            }

            if (user.PasswordResetVerifyFailedCount >= 5)
            {
                await _authRateLimitService.RegisterOtpVerifyFailedAsync(
                    email,
                    ip,
                    cancellationToken);

                return Result.Failure("Bạn nhập sai OTP quá nhiều lần. Vui lòng yêu cầu mã mới.");
            }

            var validOtp = _otpService.VerifyOtp(
                request.OtpCode,
                user.PasswordResetOtpHash);

            if (!validOtp)
            {
                user.PasswordResetVerifyFailedCount++;

                _userRepository.Update(user);

                await _userRepository.SaveChangesAsync(cancellationToken);

                await _authRateLimitService.RegisterOtpVerifyFailedAsync(
                    email,
                    ip,
                    cancellationToken);

                await WriteAuditSafeAsync(
                    action: "Reset Password Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: user.Id,
                    description: "Reset password failed because OTP is invalid.",
                    newValues: new
                    {
                        user.Id,
                        user.Email,
                        user.PasswordResetVerifyFailedCount,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "Invalid OTP"
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("OTP không đúng.");
            }

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            user.PasswordResetOtpHash = null;
            user.PasswordResetOtpExpiredAt = null;
            user.PasswordResetVerifyFailedCount = 0;

            _userRepository.Update(user);

            await _userRepository.SaveChangesAsync(cancellationToken);

            await _authRateLimitService.RegisterOtpVerifySuccessAsync(
                email,
                ip,
                cancellationToken);

            await WriteAuditSafeAsync(
                action: "Reset Password Success",
                actionType: AuditActionType.Update,
                status: AuditStatus.Success,
                entityName: "User",
                entityId: user.Id,
                description: "User reset password successfully.",
                newValues: new
                {
                    user.Id,
                    user.Email,
                    IpAddress = ip,
                    UserAgent = userAgent,
                    ResetAt = DateTime.UtcNow
                },
                cancellationToken: cancellationToken);

            return Result.Success();
        }


        public async Task<Result> ChangePasswordAsync(
                Guid userId,
                ChangePasswordRequestDto request,
                string? ipAddress,
                string? userAgent,
                CancellationToken cancellationToken = default)
        {
            if (request == null)
                return Result.Failure("Dữ liệu đổi mật khẩu không hợp lệ.");

            if (userId == Guid.Empty)
                return Result.Failure("User không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                return Result.Failure("Vui lòng nhập mật khẩu hiện tại.");

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return Result.Failure("Vui lòng nhập mật khẩu mới.");

            if (string.IsNullOrWhiteSpace(request.ConfirmNewPassword))
                return Result.Failure("Vui lòng nhập xác nhận mật khẩu mới.");

            if (request.NewPassword.Length < 6)
                return Result.Failure("Mật khẩu mới phải có ít nhất 6 ký tự.");

            if (request.NewPassword != request.ConfirmNewPassword)
                return Result.Failure("Xác nhận mật khẩu mới không khớp.");

            if (request.CurrentPassword == request.NewPassword)
                return Result.Failure("Mật khẩu mới không được trùng với mật khẩu hiện tại.");

            var ip = string.IsNullOrWhiteSpace(ipAddress)
                ? "unknown"
                : ipAddress.Trim();

            var user = await _userRepository.GetByIdWithRolesAsync(
                userId,
                cancellationToken);

            if (user == null)
            {
                await WriteAuditSafeAsync(
                    action: "Change Password Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: userId,
                    description: "Change password failed because user was not found.",
                    newValues: new
                    {
                        UserId = userId,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "User not found",
                        FailedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Không tìm thấy user.");
            }

            if (!user.IsActive)
            {
                await WriteAuditSafeAsync(
                    action: "Change Password Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: user.Id,
                    description: "Change password failed because user is inactive.",
                    newValues: new
                    {
                        user.Id,
                        user.Email,
                        user.IsActive,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "User inactive",
                        FailedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Tài khoản đã bị khóa.");
            }

            var validCurrentPassword = _passwordHasher.Verify(
                request.CurrentPassword,
                user.PasswordHash);

            if (!validCurrentPassword)
            {
                await WriteAuditSafeAsync(
                    action: "Change Password Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: user.Id,
                    description: "Change password failed because current password is invalid.",
                    newValues: new
                    {
                        user.Id,
                        user.Email,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "Invalid current password",
                        FailedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);

                return Result.Failure("Mật khẩu hiện tại không đúng.");
            }

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);

            // Nếu user đang có OTP reset password cũ thì xóa luôn để tránh dùng lại
            user.PasswordResetOtpHash = null;
            user.PasswordResetOtpExpiredAt = null;
            user.PasswordResetVerifyFailedCount = 0;

            _userRepository.Update(user);

            await _userRepository.SaveChangesAsync(cancellationToken);

            await WriteAuditSafeAsync(
                action: "Change Password Success",
                actionType: AuditActionType.Update,
                status: AuditStatus.Success,
                entityName: "User",
                entityId: user.Id,
                description: "User changed password successfully.",
                newValues: new
                {
                    user.Id,
                    user.Email,
                    IpAddress = ip,
                    UserAgent = userAgent,
                    ChangedAt = DateTime.UtcNow
                },
                cancellationToken: cancellationToken);

            return Result.Success();
        }


        public async Task<Result<Guid>> CreateAdminUserAsync(
            CreateAdminUserRequestDto request,
            Guid createdByUserId,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                return Result<Guid>.Failure("Dữ liệu tạo admin không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<Guid>.Failure("Vui lòng nhập email.");

            if (string.IsNullOrWhiteSpace(request.FullName))
                return Result<Guid>.Failure("Vui lòng nhập họ tên.");

            if (string.IsNullOrWhiteSpace(request.Password))
                return Result<Guid>.Failure("Vui lòng nhập mật khẩu.");

            if (string.IsNullOrWhiteSpace(request.ConfirmPassword))
                return Result<Guid>.Failure("Vui lòng nhập xác nhận mật khẩu.");

            if (request.Password.Length < 6)
                return Result<Guid>.Failure("Mật khẩu phải có ít nhất 6 ký tự.");

            if (request.Password != request.ConfirmPassword)
                return Result<Guid>.Failure("Xác nhận mật khẩu không khớp.");

            var email = request.Email.Trim().ToLowerInvariant();

            var ip = string.IsNullOrWhiteSpace(ipAddress)
                ? "unknown"
                : ipAddress.Trim();

            var exists = await _userRepository.ExistsByEmailAsync(
                email,
                null,
                cancellationToken);

            if (exists)
            {
                await WriteAuditSafeAsync(
                    action: "Create Admin User Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    description: "Create admin user failed because email already exists.",
                    newValues: new
                    {
                        Email = email,
                        CreatedByUserId = createdByUserId,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "Email already exists",
                        FailedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);

                return Result<Guid>.Failure("Email đã tồn tại.");
            }

            var adminRole = await _roleRepository.GetByCodeAsync(
                "ADMIN",
                cancellationToken);

            if (adminRole == null)
            {
                await WriteAuditSafeAsync(
                    action: "Create Admin User Failed",
                    actionType: AuditActionType.Create,
                    status: AuditStatus.Failed,
                    entityName: "Role",
                    description: "Create admin user failed because ADMIN role does not exist.",
                    newValues: new
                    {
                        Email = email,
                        RoleCode = "ADMIN",
                        CreatedByUserId = createdByUserId,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        Reason = "Role ADMIN not found",
                        FailedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);

                return Result<Guid>.Failure("Role ADMIN không tồn tại.");
            }

            var user = new User
            {
                Email = email,
                Phone = string.IsNullOrWhiteSpace(request.Phone)
                    ? null
                    : request.Phone.Trim(),
                FullName = request.FullName.Trim(),
                PasswordHash = _passwordHasher.Hash(request.Password),
                EmailConfirmed = true,
                IsActive = true
            };

            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = adminRole.Id
            });

            await _userRepository.AddAsync(
                user,
                cancellationToken);

            await _userRepository.SaveChangesAsync(
                cancellationToken);

            await WriteAuditSafeAsync(
                action: "Create Admin User Success",
                actionType: AuditActionType.Create,
                status: AuditStatus.Success,
                entityName: "User",
                entityId: user.Id,
                description: "Admin user was created successfully.",
                newValues: new
                {
                    user.Id,
                    user.Email,
                    user.Phone,
                    user.FullName,
                    user.EmailConfirmed,
                    user.IsActive,
                    Role = adminRole.Code,
                    CreatedByUserId = createdByUserId,
                    IpAddress = ip,
                    UserAgent = userAgent,
                    CreatedAt = DateTime.UtcNow
                },
                cancellationToken: cancellationToken);

            return Result<Guid>.Success(user.Id);
        }


        private static string BuildRegisterOtpEmailBody(string otp)
        {
            var safeOtp = System.Net.WebUtility.HtmlEncode(otp);

            return $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>Mã xác thực EZSIM</title>
</head>

<body style='margin:0;padding:0;background-color:#eef7f5;font-family:Arial,Helvetica,sans-serif;color:#111827;'>

    <!-- Preheader -->
    <div style='display:none;max-height:0;overflow:hidden;font-size:1px;line-height:1px;color:#eef7f5;'>
        Mã xác thực EZSIM của bạn là {safeOtp}. Mã có hiệu lực trong 10 phút.
    </div>

    <table width='100%' cellpadding='0' cellspacing='0' role='presentation'
           style='width:100%;background-color:#eef7f5;padding:36px 16px;'>
        <tr>
            <td align='center'>

                <table width='100%' cellpadding='0' cellspacing='0' role='presentation'
                       style='max-width:560px;background-color:#ffffff;border-radius:18px;overflow:hidden;box-shadow:0 12px 32px rgba(15,23,42,0.10);'>

                    <!-- Header -->
                    <tr>
                        <td style='background:linear-gradient(135deg,#00b894,#00cec9);padding:34px 32px;text-align:center;'>
                            <div style='font-size:30px;line-height:36px;font-weight:800;color:#ffffff;letter-spacing:-0.6px;'>
                                ezsim
                            </div>
                            <div style='margin-top:8px;font-size:14px;line-height:20px;color:#eafffb;'>
                                eSIM du lịch nhanh chóng, tiện lợi
                            </div>
                        </td>
                    </tr>

                    <!-- Title -->
                    <tr>
                        <td style='padding:34px 32px 0 32px;text-align:center;'>
                            <h1 style='margin:0;font-size:26px;line-height:34px;font-weight:800;color:#111827;'>
                                Xác thực tài khoản của bạn
                            </h1>
                        </td>
                    </tr>

                    <!-- Description -->
                    <tr>
                        <td style='padding:14px 36px 0 36px;text-align:center;'>
                            <p style='margin:0;font-size:16px;line-height:25px;color:#4b5563;'>
                                Nhập mã bên dưới để hoàn tất đăng ký tài khoản EZSIM.
                            </p>
                        </td>
                    </tr>

                    <!-- OTP Box -->
                    <tr>
                        <td style='padding:32px 32px 24px 32px;text-align:center;'>
                            <div style='
                                display:inline-block;
                                font-size:42px;
                                line-height:52px;
                                font-weight:800;
                                letter-spacing:9px;
                                color:#047857;
                                background-color:#ecfdf5;
                                border:1px solid #a7f3d0;
                                padding:18px 30px;
                                border-radius:14px;
                            '>
                                {safeOtp}
                            </div>
                        </td>
                    </tr>

                    <!-- Expired info -->
                    <tr>
                        <td style='padding:0 36px 26px 36px;text-align:center;'>
                            <p style='margin:0;font-size:14px;line-height:22px;color:#374151;'>
                                Mã này có hiệu lực trong 
                                <strong style='color:#047857;'>10 phút</strong>.
                            </p>
                        </td>
                    </tr>

                    <!-- Security note -->
                    <tr>
                        <td style='padding:0 32px 34px 32px;'>
                            <table width='100%' cellpadding='0' cellspacing='0' role='presentation'
                                   style='background-color:#f9fafb;border:1px solid #e5e7eb;border-radius:14px;'>
                                <tr>
                                    <td style='padding:18px 20px;text-align:left;'>
                                        <p style='margin:0;font-size:14px;line-height:22px;color:#4b5563;'>
                                            <strong style='color:#111827;'>Lưu ý bảo mật:</strong>
                                            Không chia sẻ mã này với bất kỳ ai. Nếu bạn không yêu cầu mã xác thực,
                                            bạn có thể bỏ qua email này.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='background-color:#f3f4f6;padding:22px 32px;text-align:center;'>
                            <p style='margin:0;font-size:12px;line-height:18px;color:#6b7280;'>
                                Email này được gửi tự động từ EZSIM. Vui lòng không trả lời email này.
                            </p>
                            <p style='margin:8px 0 0 0;font-size:12px;line-height:18px;color:#9ca3af;'>
                                © EZSIM
                            </p>
                        </td>
                    </tr>

                </table>

            </td>
        </tr>
    </table>

</body>
</html>";
        }


        private static string BuildForgotPasswordOtpEmailBody(string otp)
        {
            return $@"
        <!DOCTYPE html>
        <html lang='vi'>
        <head>
            <meta charset='UTF-8' />
            <meta name='viewport' content='width=device-width, initial-scale=1.0' />
            <title>Mã đặt lại mật khẩu DTP</title>
        </head>
        <body style='margin:0;padding:0;background-color:#f7f7f7;font-family:Arial,Helvetica,sans-serif;color:#191414;'>
            <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f7f7f7;padding:32px 0;'>
                <tr>
                    <td align='center'>
                        <table width='100%' cellpadding='0' cellspacing='0' style='max-width:520px;background-color:#ffffff;border-radius:8px;overflow:hidden;'>
                            <tr>
                                <td style='padding:32px 32px 20px 32px;text-align:center;'>
                                    <div style='font-size:28px;font-weight:700;color:#111111;letter-spacing:-0.5px;'>
                                        DTP
                                    </div>
                                </td>
                            </tr>

                            <tr>
                                <td style='padding:8px 32px 0 32px;text-align:center;'>
                                    <h1 style='margin:0;font-size:26px;line-height:34px;font-weight:700;color:#191414;'>
                                        Mã đặt lại mật khẩu của bạn
                                    </h1>
                                </td>
                            </tr>

                            <tr>
                                <td style='padding:20px 32px 0 32px;text-align:center;'>
                                    <p style='margin:0;font-size:16px;line-height:24px;color:#333333;'>
                                        Nhập mã bên dưới để đặt lại mật khẩu tài khoản DTP.
                                    </p>
                                </td>
                            </tr>

                            <tr>
                                <td style='padding:32px 32px 24px 32px;text-align:center;'>
                                    <div style='
                                        display:inline-block;
                                        font-size:42px;
                                        line-height:52px;
                                        font-weight:700;
                                        letter-spacing:8px;
                                        color:#191414;
                                        background-color:#f1f1f1;
                                        padding:16px 28px;
                                        border-radius:6px;
                                    '>
                                        {otp}
                                    </div>
                                </td>
                            </tr>

                            <tr>
                                <td style='padding:0 32px 28px 32px;text-align:center;'>
                                    <p style='margin:0;font-size:14px;line-height:22px;color:#555555;'>
                                        Mã này có hiệu lực trong <strong>10 phút</strong>.
                                    </p>
                                </td>
                            </tr>

                            <tr>
                                <td style='padding:0 32px 32px 32px;text-align:center;'>
                                    <p style='margin:0;font-size:14px;line-height:22px;color:#555555;'>
                                        Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này.
                                        Không chia sẻ mã này với bất kỳ ai.
                                    </p>
                                </td>
                            </tr>

                            <tr>
                                <td style='background-color:#f7f7f7;padding:24px 32px;text-align:center;'>
                                    <p style='margin:0;font-size:12px;line-height:18px;color:#888888;'>
                                        Email này được gửi tự động từ DTP. Vui lòng không trả lời email này.
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>";
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
