using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Infrastructure.Repositories
{
    public class ProviderRepository
     : RepositoryBase<Provider>,
       IProviderRepository
    {
        private readonly CatalogDbContext _context;

        public ProviderRepository(CatalogDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Provider>> GetListAsync(
            string? keyword,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Providers
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Code.Contains(keyword) ||
                    x.Name.Contains(keyword));
            }

            return await query
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            code = code.Trim();

            return await _context.Providers.AnyAsync(x =>
                x.Code == code &&
                (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(
            string name,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            name = name.Trim();

            return await _context.Providers.AnyAsync(x =>
                x.Name == name &&
                (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
        }


        public async Task<PagedResultDto<ProviderDto>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Providers
                .Where(x => !x.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Name.Contains(keyword) ||
                    x.Code!.Contains(keyword));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProviderDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    //Slug = x.Slug,
                    //LogoUrl = x.LogoUrl,
                    //WebsiteUrl = x.WebsiteUrl,
                    ApiBaseUrl = x.ApiBaseUrl,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<ProviderDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
