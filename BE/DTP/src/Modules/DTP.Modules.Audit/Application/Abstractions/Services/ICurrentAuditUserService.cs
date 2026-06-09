using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Application.Abstractions.Services
{
    public interface ICurrentAuditUserService
    {
        Guid? UserId { get; }

        string? UserName { get; }

        string? IpAddress { get; }

        string? UserAgent { get; }

        string? RequestPath { get; }

        string? RequestMethod { get; }

        string? CorrelationId { get; }
    }
}
