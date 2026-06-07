using DTP.Modules.Provider.Domain.Entities;
using DTP.Modules.Provider.Domain.Enums;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Repositories
{
    public interface IProviderApiLogRepository
    {
        Task<ProviderApiLog?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<ProviderApiLogDto>> GetPagedAsync(
            Guid? providerId,
            ProviderApiLogType? logType,
            bool? isSuccess,
            DateTime? fromDate,
            DateTime? toDate,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProviderApiLog log,
            CancellationToken cancellationToken = default);

        void Update(ProviderApiLog log);

        void Remove(ProviderApiLog log);
    }
}
