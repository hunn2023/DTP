using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Infrastructure.Repositories
{
    public class ContentFaqRepository : IContentFaqRepository
    {
        private readonly ContentDbContext _dbContext;

        public ContentFaqRepository(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ContentFaq?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.ContentFaqs
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<(IReadOnlyList<ContentFaq> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            string? categoryCode,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ContentFaqs
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Question.Contains(keyword) ||
                    x.Answer.Contains(keyword) ||
                    (x.CategoryCode != null && x.CategoryCode.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(categoryCode))
            {
                categoryCode = categoryCode.Trim().ToUpperInvariant();

                query = query.Where(x => x.CategoryCode == categoryCode);
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<ContentFaq>> GetActiveAsync(
            string? categoryCode,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ContentFaqs
                .AsNoTracking()
                .Where(x => x.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoryCode))
            {
                categoryCode = categoryCode.Trim().ToUpperInvariant();

                query = query.Where(x => x.CategoryCode == categoryCode);
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public Task AddAsync(
            ContentFaq faq,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.ContentFaqs
                .AddAsync(faq, cancellationToken)
                .AsTask();
        }

        public void Update(ContentFaq faq)
        {
            _dbContext.ContentFaqs.Update(faq);
        }
    }
}
