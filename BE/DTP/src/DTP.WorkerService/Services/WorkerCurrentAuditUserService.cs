using DTP.Modules.Audit.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.WorkerService.Services
{
    public class WorkerCurrentAuditUserService : ICurrentAuditUserService
    {
        public Guid? UserId => null;

        public string? UserName => "system-worker";

        public string? IpAddress => "worker";

        public string? UserAgent => "DTP.WorkerService";

        public string? RequestPath => "background-worker/provider-redeem-polling";

        public string? RequestMethod => "WORKER";

        public string? CorrelationId => Guid.NewGuid().ToString("N");
    }
}
