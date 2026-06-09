using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Abstractions.Caching
{
    public interface IReportCacheInvalidator
    {
        Task ClearAllAsync(CancellationToken cancellationToken = default);
    }
}
