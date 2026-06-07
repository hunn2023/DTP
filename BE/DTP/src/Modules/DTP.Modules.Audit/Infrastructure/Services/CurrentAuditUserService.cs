using DTP.Modules.Audit.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Infrastructure.Services
{
    public class CurrentAuditUserService : ICurrentAuditUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentAuditUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdValue =
                    HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? HttpContext?.User?.FindFirstValue("sub")
                    ?? HttpContext?.User?.FindFirstValue("userId");

                if (Guid.TryParse(userIdValue, out var userId))
                    return userId;

                return null;
            }
        }

        public string? UserName
        {
            get
            {
                return HttpContext?.User?.FindFirstValue(ClaimTypes.Name)
                    ?? HttpContext?.User?.FindFirstValue("name")
                    ?? HttpContext?.User?.FindFirstValue("email");
            }
        }

        public string? IpAddress
        {
            get
            {
                var forwardedFor =
                    HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(forwardedFor))
                    return forwardedFor.Split(',').FirstOrDefault();

                return HttpContext?.Connection.RemoteIpAddress?.ToString();
            }
        }

        public string? UserAgent =>
            HttpContext?.Request.Headers.UserAgent.ToString();

        public string? RequestPath =>
            HttpContext?.Request.Path.Value;

        public string? RequestMethod =>
            HttpContext?.Request.Method;

        public string? CorrelationId =>
            HttpContext?.TraceIdentifier;

        private HttpContext? HttpContext =>
            _httpContextAccessor.HttpContext;
    }
}
