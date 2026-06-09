using DTP.Modules.Report.Application.Abstractions.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Infrastructure.Caching
{
    public class ReportCacheInvalidator : IReportCacheInvalidator
    {
        public Task ClearAllAsync(CancellationToken cancellationToken = default)
        {
            // Nếu bạn đã có ICacheService hỗ trợ remove pattern thì xử lý tại đây.
            // Ví dụ:
             //await _cache.RemoveByPatternAsync("report:*", cancellationToken);

            return Task.CompletedTask;
        }
    }
}
