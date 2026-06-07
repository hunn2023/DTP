using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Modules.Provider.Domain.Enums;
using DTP.Modules.Provider.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Repositories
{
    public class ProviderProductMappingRepository : IProviderProductMappingRepository
    {
        private readonly ProviderDbContext _context;

        public ProviderProductMappingRepository(ProviderDbContext context)
        {
            _context = context;
        }

        public async Task<ProviderProductMapping?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProviderProductMappings
                .Include(x => x.Provider)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<ProviderProductMapping?> GetActiveMappingAsync(
            ProviderProductType productType,
            Guid productId,
            Guid productVariantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProviderProductMappings
                .Include(x => x.Provider)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    x.ProductType == productType &&
                    x.ProductId == productId &&
                    x.ProductVariantId == productVariantId,
                    cancellationToken);
        }

        public async Task<bool> ExistsMappingAsync(
            Guid providerId,
            Guid productVariantId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProviderProductMappings
                .AnyAsync(x =>
                    !x.IsDeleted &&
                    x.ProviderId == providerId &&
                    x.ProductVariantId == productVariantId &&
                    (!excludeId.HasValue || x.Id != excludeId.Value),
                    cancellationToken);
        }

        public async Task AddAsync(
            ProviderProductMapping mapping,
            CancellationToken cancellationToken = default)
        {
            await _context.ProviderProductMappings.AddAsync(mapping, cancellationToken);
        }

        public void Update(ProviderProductMapping mapping)
        {
            _context.ProviderProductMappings.Update(mapping);
        }

        public void Remove(ProviderProductMapping mapping)
        {
            mapping.Delete();
            _context.ProviderProductMappings.Update(mapping);
        }


        public async Task<PagedResultDto<ProviderProductMappingDto>> GetPagedAsync(
    Guid? providerId,
    ProviderProductType? productType,
    Guid? productId,
    Guid? productVariantId,
    bool? isActive,
    int pageIndex,
    int pageSize,
    CancellationToken cancellationToken = default)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 20;

            var query = _context.ProviderProductMappings
                .AsNoTracking()
                .Include(x => x.Provider)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (providerId.HasValue && providerId.Value != Guid.Empty)
            {
                query = query.Where(x => x.ProviderId == providerId.Value);
            }

            if (productType.HasValue)
            {
                query = query.Where(x => x.ProductType == productType.Value);
            }

            if (productId.HasValue && productId.Value != Guid.Empty)
            {
                query = query.Where(x => x.ProductId == productId.Value);
            }

            if (productVariantId.HasValue && productVariantId.Value != Guid.Empty)
            {
                query = query.Where(x => x.ProductVariantId == productVariantId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProviderProductMappingDto
                {
                    Id = x.Id,
                    ProviderId = x.ProviderId,
                    ProviderName = x.Provider.Name,
                    ProductType = x.ProductType,
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    ProviderProductCode = x.ProviderProductCode,
                    ProviderProductName = x.ProviderProductName,
                    ProviderCostPrice = x.ProviderCostPrice,
                    CurrencyCode = x.CurrencyCode,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<ProviderProductMappingDto>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }
}
