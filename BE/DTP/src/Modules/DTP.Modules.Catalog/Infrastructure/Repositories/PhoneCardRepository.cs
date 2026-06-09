using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Commands.PhoneCards;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Catalog.Infrastructure.Repositories
{
    public class PhoneCardRepository
       : RepositoryBase<PhoneCard>,
         IPhoneCardRepository
    {
        private readonly CatalogDbContext _context;

        public PhoneCardRepository(CatalogDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<PhoneCardDto>> GetPublicPagedAsync(
            Guid? providerId,
            decimal? minFaceValue,
            decimal? maxFaceValue,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.PhoneCards
                .AsNoTracking()
                .Where(x => x.IsActive)
                .AsQueryable();

            query = ApplyFilters(
                query,
                keyword: null,
                productVariantId: null,
                providerId,
                minFaceValue,
                maxFaceValue,
                isActive: null);

            return await ToPagedDtoAsync(
                query,
                pageIndex,
                pageSize,
                cancellationToken);
        }

        public async Task<PhoneCardDto?> GetPublicBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            slug = slug.Trim().ToLower();

            return await _context.PhoneCards
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.Slug == slug)
                .Select(x => new PhoneCardDto
                {
                    Id = x.Id,
                    ProductVariantId = x.ProductVariantId,
                    ProductVariantName = x.ProductVariant.Name,
                    ProviderId = x.ProviderId,
                    ProviderName = x.Provider.Name,
                    Name = x.Name,
                    Slug = x.Slug,
                    FaceValue = x.FaceValue,
                    Price = x.Price,
                    Currency = x.Currency,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedResultDto<PhoneCardDto>> GetPagedAsync(
            string? keyword,
            Guid? productVariantId,
            Guid? providerId,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.PhoneCards
                .AsNoTracking()
                .AsQueryable();

            query = ApplyFilters(
                query,
                keyword,
                productVariantId,
                providerId,
                minFaceValue: null,
                maxFaceValue: null,
                isActive);

            return await ToPagedDtoAsync(
                query,
                pageIndex,
                pageSize,
                cancellationToken);
        }

        public async Task<PhoneCardDto?> GetByIdDtoAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.PhoneCards
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new PhoneCardDto
                {
                    Id = x.Id,
                    ProductVariantId = x.ProductVariantId,
                    ProductVariantName = x.ProductVariant.Name,
                    ProviderId = x.ProviderId,
                    ProviderName = x.Provider.Name,
                    Name = x.Name,
                    Slug = x.Slug,
                    FaceValue = x.FaceValue,
                    Price = x.Price,
                    Currency = x.Currency,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Guid> CreateAsync(
            CreatePhoneCardCommand command,
            CancellationToken cancellationToken = default)
        {
            var phoneCard = new PhoneCard(
                command.ProductVariantId,
                command.ProviderId,
                command.Name,
                command.Slug,
                command.FaceValue,
                command.Price,
                command.Currency,
                command.SortOrder);

            if (!command.IsActive)
            {
                phoneCard.Update(
                    command.ProviderId,
                    command.Name,
                    command.Slug,
                    command.FaceValue,
                    command.Price,
                    command.Currency,
                    command.SortOrder,
                    false);
            }

            _context.PhoneCards.Add(phoneCard);

            await _context.SaveChangesAsync(cancellationToken);

            return phoneCard.Id;
        }

        public async Task<bool> UpdateAsync(
            UpdatePhoneCardCommand command,
            CancellationToken cancellationToken = default)
        {
            var phoneCard = await _context.PhoneCards
                .FirstOrDefaultAsync(
                    x => x.Id == command.Id,
                    cancellationToken);

            if (phoneCard is null)
            {
                return false;
            }

            phoneCard.Update(
                command.ProviderId,
                command.Name,
                command.Slug,
                command.FaceValue,
                command.Price,
                command.Currency,
                command.SortOrder,
                command.IsActive);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var phoneCard = await _context.PhoneCards
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (phoneCard is null)
            {
                return false;
            }

            _context.PhoneCards.Remove(phoneCard);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static IQueryable<PhoneCard> ApplyFilters(
            IQueryable<PhoneCard> query,
            string? keyword,
            Guid? productVariantId,
            Guid? providerId,
            decimal? minFaceValue,
            decimal? maxFaceValue,
            bool? isActive)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Name.Contains(keyword) ||
                    x.Slug.Contains(keyword) ||
                    x.Provider.Name.Contains(keyword));
            }

            if (productVariantId.HasValue)
            {
                query = query.Where(x =>
                    x.ProductVariantId == productVariantId.Value);
            }

            if (providerId.HasValue)
            {
                query = query.Where(x =>
                    x.ProviderId == providerId.Value);
            }

            if (minFaceValue.HasValue)
            {
                query = query.Where(x =>
                    x.FaceValue >= minFaceValue.Value);
            }

            if (maxFaceValue.HasValue)
            {
                query = query.Where(x =>
                    x.FaceValue <= maxFaceValue.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(x =>
                    x.IsActive == isActive.Value);
            }

            return query;
        }

        private static async Task<PagedResultDto<PhoneCardDto>> ToPagedDtoAsync(
            IQueryable<PhoneCard> query,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.FaceValue)
                .Select(x => new PhoneCardDto
                {
                    Id = x.Id,
                    ProductVariantId = x.ProductVariantId,
                    ProductVariantName = x.ProductVariant.Name,
                    ProviderId = x.ProviderId,
                    ProviderName = x.Provider.Name,
                    Name = x.Name,
                    Slug = x.Slug,
                    FaceValue = x.FaceValue,
                    Price = x.Price,
                    Currency = x.Currency,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive
                })
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<PhoneCardDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
