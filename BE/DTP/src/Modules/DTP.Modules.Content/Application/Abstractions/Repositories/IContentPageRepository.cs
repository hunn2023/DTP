using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Abstractions.Repositories
{
    public interface IContentPageRepository
    {
        Task<ContentPage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ContentPage?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<bool> ExistsCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> ExistsSlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<ContentPage> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            ContentPageStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ContentPage>> GetPublishedAsync(CancellationToken cancellationToken = default);

        Task AddAsync(ContentPage page, CancellationToken cancellationToken = default);
        void Update(ContentPage page);
    }
}
