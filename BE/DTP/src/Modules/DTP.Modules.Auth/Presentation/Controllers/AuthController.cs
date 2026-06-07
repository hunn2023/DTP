
using DTP.Modules.Auth.Application.Commands.Auths;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Modules.Auth.Application.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var result = await _mediator.Send(new RegisterCommand
            {
                Request = request,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            });

            return Ok(new
            {
                success = result,
                message = "OTP đã được gửi tới email."
            });
        }

        [HttpPost("verify-register-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyRegisterOtp(VerifyOtpRequestDto request)
        {
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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await _mediator.Send(new LoginCommand
            {
                Request = request,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _mediator.Send(new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
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
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
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
    }
}