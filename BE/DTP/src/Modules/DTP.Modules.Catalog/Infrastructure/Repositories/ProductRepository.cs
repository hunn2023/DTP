using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Commands.Products;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Catalog.Infrastructure.Repositories
{
    public class ProductRepository
         : RepositoryBase<Product>,
           IProductRepository
    {
        private readonly CatalogDbContext _context;

        public ProductRepository(CatalogDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Product?> GetDetailAsync(
           Guid id,
           CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Country)
                .Include(x => x.Images)
                .Include(x => x.Variants)
                .Include(x => x.Attributes)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<PagedResultDto<ProductDto>> GetPublicPagedAsync(
             string? keyword,
             Guid? categoryId,
             Guid? countryId,
             int pageIndex,
             int pageSize,
             CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(x => x.IsActive)
                .AsQueryable();

            query = ApplyFilters(
                query,
                keyword,
                categoryId,
                countryId,
                null);

            return await ToPagedDtoAsync(
                query,
                pageIndex,
                pageSize,
                cancellationToken);
        }


        public async Task<ProductDto?> GetPublicBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            slug = slug.Trim().ToLower();

            return await _context.Products
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.Slug == slug)
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Slug = x.Slug,
                    CategoryId = x.CategoryId,
                    //CategoryName = x.Category.Name,
                    ShortDescription = x.ShortDescription,
                    Description = x.Description,
                    ThumbnailUrl = x.ThumbnailUrl,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<PagedResultDto<ProductDto>> GetPagedAsync(
            string? keyword,
            Guid? categoryId,
            Guid? countryId,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                .AsNoTracking()
                .AsQueryable();

            query = ApplyFilters(
                query,
                keyword,
                categoryId,
                countryId,
                isActive);

            return await ToPagedDtoAsync(
                query,
                pageIndex,
                pageSize,
                cancellationToken);
        }



        public async Task<ProductDto?> GetByIdDtoAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Slug = x.Slug,
                    CategoryId = x.CategoryId,
                    //CategoryName = x.Category.Name,
                    ShortDescription = x.ShortDescription,
                    Description = x.Description,
                    ThumbnailUrl = x.ThumbnailUrl,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Guid> CreateAsync(
            CreateProductCommand command,
            CancellationToken cancellationToken = default)
        {
            var product = new Product(
                command.Code?.Trim(),
                command.Name.Trim(),
                command.Slug.Trim().ToLower(),
                command.CategoryId,
                command.CountryId,
                command.ShortDescription?.Trim(),
                command.Description?.Trim(),
                command.LocationText?.Trim(),
                command.ThumbnailUrl?.Trim(),
                command.IsFeatured,
                command.IsHot,
                command.SortOrder,
                command.IsActive);

            _context.Products.Add(product);

            await _context.SaveChangesAsync(cancellationToken);

            return product.Id;
        }

        public async Task<bool> UpdateAsync(
            UpdateProductCommand command,
            CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(
                    x => x.Id == command.Id,
                    cancellationToken);

            if (product is null)
                return false;

            product.Update(
                command.Code?.Trim(),
                command.Name.Trim(),
                command.Slug.Trim().ToLower(),
                command.CategoryId,
                command.CountryId,
                command.ShortDescription?.Trim(),
                command.Description?.Trim(),
                command.LocationText?.Trim(),
                command.IsFeatured,
                command.IsHot,
                command.SortOrder,
                command.IsActive);

            product.UpdateThumbnail(command.ThumbnailUrl?.Trim());

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (product is null)
            {
                return false;
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static IQueryable<Product> ApplyFilters(
                 IQueryable<Product> query,
                 string? keyword,
                 Guid? categoryId,
                 Guid? countryId,
                 bool? isActive,
                 bool? isFeatured = null,
                 bool? isHot = null)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Name.Contains(keyword) ||
                    x.Slug.Contains(keyword) ||
                    (x.Code != null && x.Code.Contains(keyword)) ||
                    (x.LocationText != null && x.LocationText.Contains(keyword)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (countryId.HasValue)
            {
                query = query.Where(x => x.CountryId == countryId.Value);
            }

            if (countryId.HasValue)
            {
                var productIdsQuery =
                    from esim in _context.EsimPackages
                    join variant in _context.ProductVariants
                        on esim.ProductVariantId equals variant.Id
                    where esim.CountryId == countryId.Value
                    select variant.ProductId;

                query = query.Where(product => productIdsQuery.Contains(product.Id));
            }

            if (carrierId.HasValue)
            {
                var productIdsQuery =
                    from esim in _context.EsimPackages
                    join variant in _context.ProductVariants
                        on esim.ProductVariantId equals variant.Id
                    where esim.CarrierId == carrierId.Value
                    select variant.ProductId;

                query = query.Where(product => productIdsQuery.Contains(product.Id));
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            if (isFeatured.HasValue)
            {
                query = query.Where(x => x.IsFeatured == isFeatured.Value);
            }

            if (isHot.HasValue)
            {
                query = query.Where(x => x.IsHot == isHot.Value);
            }

            return query;
        }

        private static async Task<PagedResultDto<ProductDto>> ToPagedDtoAsync(
            IQueryable<Product> query,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
               .Select(x => new ProductDto
               {
                   Id = x.Id,
                   Code = x.Code,
                   Name = x.Name,
                   Slug = x.Slug,

                   CategoryId = x.CategoryId,
                   CategoryName = x.Category != null ? x.Category.Name : null,

                   CountryId = x.CountryId,
                   CountryName = x.Country != null ? x.Country.Name : null,

                   ShortDescription = x.ShortDescription,
                   Description = x.Description,
                   LocationText = x.LocationText,
                   ThumbnailUrl = x.ThumbnailUrl,

                   IsFeatured = x.IsFeatured,
                   IsHot = x.IsHot,
                   SoldCount = x.SoldCount,

                   SortOrder = x.SortOrder,
                   IsActive = x.IsActive
               })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<ProductDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
