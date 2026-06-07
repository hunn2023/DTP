using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Abstractions.Repositories
{
    public interface IContentBannerRepository
    {
        Task<ContentBanner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<ContentBanner> Items, int TotalCount)> GetPagedAsync(
            ContentBannerPosition? position,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ContentBanner>> GetAvailableAsync(
            ContentBannerPosition? position,
            DateTime now,
            CancellationToken cancellationToken = default);

        Task AddAsync(ContentBanner banner, CancellationToken cancellationToken = default);
        void Update(ContentBanner banner);
    }
}
