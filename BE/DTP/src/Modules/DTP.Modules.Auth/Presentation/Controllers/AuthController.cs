using DTP.Modules.Auth.Application.Commands.Auths;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Modules.Auth.Application.Queries.Users;
using DTP.Shared.Application;
using DTP.Shared.Application.Http;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace DTP.Modules.Auth.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [EnableRateLimiting("auth-register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {

            var result = await _mediator.Send(new RegisterCommand
            {
                Request = request,
                IpAddress = HttpContext.GetClientIp(),
                UserAgent = HttpContext.GetUserAgent()
            });

            return Ok(new
            {
                success = result,
                message = "OTP đã được gửi tới email."
            });
        }

        [HttpPost("verify-register-otp")]
        [AllowAnonymous]
        [EnableRateLimiting("auth-otp")]
        public async Task<IActionResult> VerifyRegisterOtp(VerifyOtpRequestDto request)
        {
            request.IpAddress = HttpContext.GetClientIp();
            request.UserAgent = Request.Headers.UserAgent.ToString();
            var result = await _mediator.Send(new VerifyRegisterOtpCommand
            {
                Request = request
            });

            return Ok(new
            {
                success = result,
                message = "Đăng ký thành công."
            });
        }

        [EnableRateLimiting("auth-login")]
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await _mediator.Send(new LoginCommand
            {
                Request = request,
                IpAddress = HttpContext.GetClientIp()
            });

            return Ok(result);
        }


        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [EnableRateLimiting("auth-refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _mediator.Send(new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken,
                IpAddress = HttpContext.GetClientIp()
            });

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(RefreshTokenRequestDto request)
        {
            var result = await _mediator.Send(new LogoutCommand
            {
                RefreshToken = request.RefreshToken,
                IpAddress = HttpContext.GetClientIp()
            });

            return Ok(new
            {
                success = result
            });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdValue))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new GetProfileQuery
            {
                UserId = Guid.Parse(userIdValue)
            });

            return Ok(result);
        }



        [EnableRateLimiting("auth-otp")]
        [HttpPost("register/resend-otp")]
        public async Task<IActionResult> ResendRegisterOtp(
            [FromBody] ResendRegisterOtpRequestDto request,
            CancellationToken cancellationToken)
        {
            var ip = HttpContext.GetClientIp();
            var userAgent = HttpContext.GetUserAgent();

            var command = new ResendRegisterOtpCommand(
                request.Email,
                ip,
                userAgent);

            var result = await _mediator.Send(
                command,
                cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
                [FromBody] ForgotPasswordCommand command,
                CancellationToken cancellationToken)
        {
            command.IpAddress = HttpContext.GetClientIp();
            command.UserAgent = HttpContext.GetUserAgent();

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
                [FromBody] ResetPasswordCommand command,
                CancellationToken cancellationToken)
        {
            command.IpAddress = HttpContext.GetClientIp();
            command.UserAgent = HttpContext.GetUserAgent();

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }



        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordCommand command,
            CancellationToken cancellationToken)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? User.FindFirstValue("userId");

            if (!Guid.TryParse(userIdValue, out var userId))
                return Unauthorized(Result.Failure("Token không hợp lệ."));

            command.UserId = userId;
            command.IpAddress = HttpContext.GetClientIp();
            command.UserAgent = HttpContext.GetUserAgent();

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}