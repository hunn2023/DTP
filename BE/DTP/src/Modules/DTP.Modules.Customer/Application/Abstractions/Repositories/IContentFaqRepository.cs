using DTP.Modules.Content.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Abstractions.Repositories
{
    public interface IContentFaqRepository
    {
        Task<ContentFaq?> GetByIdAsync(
       Guid id,
       CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<ContentFaq> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            string? categoryCode,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ContentFaq>> GetActiveAsync(
            string? categoryCode,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ContentFaq faq,
            CancellationToken cancellationToken = default);

        void Update(ContentFaq faq);
    }
}
