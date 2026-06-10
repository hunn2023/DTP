using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Http
{
    public static class HttpContextExtensions
    {
        public static string GetClientIp(this HttpContext? httpContext)
        {
            if (httpContext == null)
                return "unknown";

            var cfConnectingIp = httpContext.Request.Headers["CF-Connecting-IP"]
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(cfConnectingIp))
                return cfConnectingIp.Trim();

            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"]
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                return forwardedFor
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault()
                    ?.Trim() ?? "unknown";
            }

            var realIp = httpContext.Request.Headers["X-Real-IP"]
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(realIp))
                return realIp.Trim();

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        public static string? GetUserAgent(this HttpContext? httpContext)
        {
            return httpContext?
                .Request
                .Headers["User-Agent"]
                .FirstOrDefault();
        }

        public static string? GetRequestPath(this HttpContext? httpContext)
        {
            return httpContext?
                .Request
                .Path
                .Value;
        }

        public static string? GetRequestMethod(this HttpContext? httpContext)
        {
            return httpContext?
                .Request
                .Method;
        }

        public static object GetRequestAuditInfo(this HttpContext? httpContext)
        {
            return new
            {
                IpAddress = httpContext.GetClientIp(),
                UserAgent = httpContext.GetUserAgent(),
                Path = httpContext.GetRequestPath(),
                Method = httpContext.GetRequestMethod(),
                ActionAt = DateTime.UtcNow
            };
        }
    }
}
