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
                    HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? HttpContext?.User?.FindFirst("sub")?.Value
                    ?? HttpContext?.User?.FindFirst("userId")?.Value;

                if (Guid.TryParse(userIdValue, out var userId))
                    return userId;

                return null;
            }
        }

        public string? UserName
        {
            get
            {
                return HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
                    ?? HttpContext?.User?.FindFirst("name")?.Value
                    ?? HttpContext?.User?.FindFirst("email")?.Value;
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
            HttpContext?.Request.Headers["User-Agent"].ToString();

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
