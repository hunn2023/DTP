using DTP.Modules.Provider.Application.DTOs;
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
    public interface IProviderOrderRepository
    {
        Task<ProviderOrder?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<ProviderOrder?> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<ProviderOrderDto>> GetPagedAsync(
            Guid? providerId,
            ProviderOrderStatus? status,
            string? keyword,
            DateTime? fromDate,
            DateTime? toDate,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProviderOrder order,
            CancellationToken cancellationToken = default);

        void Update(ProviderOrder order);
    }
}
