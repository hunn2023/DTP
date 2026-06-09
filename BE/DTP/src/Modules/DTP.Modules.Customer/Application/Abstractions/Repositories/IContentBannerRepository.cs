using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Abstractions.Repositories
{
    public interface IContentBannerRepository : IRepositoryBase<ContentBanner>
    {

        Task<(IReadOnlyList<ContentBanner> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            ContentBannerPosition? position,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);


        Task<IReadOnlyList<ContentBanner>> GetAvailableAsync(
            ContentBannerPosition? position,
            DateTime now,
            CancellationToken cancellationToken = default);
    }
}
