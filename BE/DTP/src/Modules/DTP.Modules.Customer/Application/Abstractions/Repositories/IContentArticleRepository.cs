using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Abstractions.Repositories
{
    public interface IContentArticleRepository
    {
        Task<ContentArticle?> GetByIdAsync(
       Guid id,
       CancellationToken cancellationToken = default);

        Task<ContentArticle?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default);

        Task<bool> ExistsSlugAsync(
            string slug,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<ContentArticle> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            string? categoryCode,
            ContentArticleStatus? status,
            bool? isFeatured,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<ContentArticle> Items, int TotalCount)> GetPublicPagedAsync(
            string? keyword,
            string? categoryCode,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ContentArticle>> GetFeaturedAsync(
            int take,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ContentArticle article,
            CancellationToken cancellationToken = default);

        void Update(ContentArticle article);

        Task<ContentArticle?> GetBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsSlugAsync(
            string slug,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<ContentArticle> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            string? categoryCode,
            ContentArticleStatus? status,
            bool? isFeatured,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<ContentArticle> Items, int TotalCount)> GetPublicPagedAsync(
            string? keyword,
            string? categoryCode,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ContentArticle>> GetFeaturedAsync(
            int take,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ContentArticle article,
            CancellationToken cancellationToken = default);

        void Update(ContentArticle article);
    }
}
