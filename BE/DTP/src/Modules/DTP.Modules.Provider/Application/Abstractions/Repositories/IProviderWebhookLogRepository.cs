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
    public interface IProviderWebhookLogRepository
    {
        Task<ProviderWebhookLog?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<ProviderWebhookLogDto>> GetPagedAsync(
            Guid? providerId,
            ProviderWebhookStatus? status,
            DateTime? fromDate,
            DateTime? toDate,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProviderWebhookLog log,
            CancellationToken cancellationToken = default);

        void Update(ProviderWebhookLog log);
    }
}
