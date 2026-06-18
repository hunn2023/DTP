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


        public async Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
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

            var priceQuery = _context.ProductPrices.AsNoTracking();


            return await ToPagedDtoAsync(
                    query,
                    priceQuery,
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
                    !x.IsDeleted &&
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
                    IsActive = x.IsActive,
                    Attributes = x.Attributes
                                .Where(a => a.IsVisible && !x.IsDeleted)
                                .OrderBy(a => a.SortOrder)
                                .Select(a => new ProductAttributeDto
                                {
                                    Key = a.Key,
                                    Value = a.Value,
                                })
                                .ToList()
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
                 .Where(x => !x.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            query = ApplyFilters(
                query,
                keyword,
                categoryId,
                countryId,
                isActive);

            var priceQuery = _context.ProductPrices.Where(x => !x.IsDeleted).AsNoTracking();


            return await ToPagedDtoAsync(
                    query,
                    priceQuery,
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
             IQueryable<ProductPrice> productPrices,
             int pageIndex,
             int pageSize,
             CancellationToken cancellationToken)
        {
            const int defaultPageIndex = 1;
            const int defaultPageSize = 20;
            const int maxPageSize = 100;

            pageIndex = pageIndex <= 0 ? defaultPageIndex : pageIndex;
            pageSize = pageSize <= 0 ? defaultPageSize : pageSize;
            pageSize = pageSize > maxPageSize ? maxPageSize : pageSize;

            var now = DateTime.UtcNow;

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

                    OriginalPrice = productPrices
                        .Where(p =>
                            p.ProductId == x.Id &&
                            p.IsActive &&
                            p.ProductVariantId == null &&
                            (p.StartDate == null || p.StartDate <= now) &&
                            (p.EndDate == null || p.EndDate >= now))
                        .OrderBy(p => p.SalePrice)
                        .Select(p => (decimal?)p.OriginalPrice)
                        .FirstOrDefault(),

                    SalePrice = productPrices
                        .Where(p =>
                            p.ProductId == x.Id &&
                            p.IsActive &&
                            p.ProductVariantId == null &&
                            (p.StartDate == null || p.StartDate <= now) &&
                            (p.EndDate == null || p.EndDate >= now))
                        .OrderBy(p => p.SalePrice)
                        .Select(p => (decimal?)p.SalePrice)
                        .FirstOrDefault(),

                    MinPrice = productPrices
                        .Where(p =>
                            p.ProductId == x.Id &&
                            p.IsActive &&
                            (p.StartDate == null || p.StartDate <= now) &&
                            (p.EndDate == null || p.EndDate >= now))
                        .OrderBy(p => p.SalePrice)
                        .Select(p => (decimal?)p.SalePrice)
                        .FirstOrDefault(),

                    Currency = productPrices
                        .Where(p =>
                            p.ProductId == x.Id &&
                            p.IsActive &&
                            (p.StartDate == null || p.StartDate <= now) &&
                            (p.EndDate == null || p.EndDate >= now))
                        .OrderBy(p => p.SalePrice)
                        .Select(p => p.Currency)
                        .FirstOrDefault(),

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


        public async Task<PagedResultDto<ProductVariantPublicDto>> GetPublicVariantPagedAsync(
                string? keyword,
                Guid? categoryId,
                Guid? countryId,
                int pageIndex,
                int pageSize,
                CancellationToken cancellationToken = default)
        {
            var variantQuery = _context.ProductVariants
                .AsNoTracking()
                .Where(v =>
                    v.IsActive && !v.IsDeleted &&
                    v.Product.IsActive && !v.Product.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                variantQuery = variantQuery.Where(v =>
                    v.Product.Name.Contains(keyword) ||
                    v.Product.Slug.Contains(keyword) ||
                    v.Name.Contains(keyword) ||
                    v.Sku!.Contains(keyword));
            }

            if (categoryId.HasValue && categoryId.Value != Guid.Empty)
            {
                variantQuery = variantQuery.Where(v =>
                    v.Product.CategoryId == categoryId.Value);
            }

            if (countryId.HasValue && countryId.Value != Guid.Empty)
            {
                variantQuery = variantQuery.Where(v =>
                    v.Product.CountryId == countryId.Value);
            }

            var priceQuery = _context.ProductPrices
                .AsNoTracking();
            var productVariantFeaturesQuery = _context.ProductVariantFeatures
                .AsNoTracking();

            return await ToPagedVariantDtoAsync(
                variantQuery,
                priceQuery,
                productVariantFeaturesQuery,
                pageIndex,
                pageSize,
                cancellationToken);
        }



        private static async Task<PagedResultDto<ProductVariantPublicDto>> ToPagedVariantDtoAsync(
             IQueryable<ProductVariant> variantQuery,
             IQueryable<ProductPrice> productPrices,
             IQueryable<ProductVariantFeature> productVariantFeatures,
             int pageIndex,
             int pageSize,
             CancellationToken cancellationToken)
        {
            const int defaultPageIndex = 1;
            const int defaultPageSize = 20;
            const int maxPageSize = 100;

            pageIndex = pageIndex <= 0 ? defaultPageIndex : pageIndex;
            pageSize = pageSize <= 0 ? defaultPageSize : pageSize;
            pageSize = pageSize > maxPageSize ? maxPageSize : pageSize;

            var now = DateTime.UtcNow;

            var query =
                from variant in variantQuery
                let price = productPrices
                    .Where(p =>
                        p.ProductId == variant.ProductId &&
                        p.ProductVariantId == variant.Id &&
                        p.IsActive &&
                        (p.StartDate == null || p.StartDate <= now) &&
                        (p.EndDate == null || p.EndDate >= now))
                    .OrderBy(p => p.SalePrice)
                    .FirstOrDefault()
                where price != null
                select new ProductVariantPublicDto
                {
                    ProductId = variant.ProductId,
                    ProductCode = variant.Product.Code,
                    ProductName = variant.Product.Name,
                    ProductSlug = variant.Product.Slug,

                    ProductVariantId = variant.Id,
                    Sku = variant.Sku,
                    VariantName = variant.Name,
                    VariantShortName = variant.ShortName,
                    VariantDescription = variant.Description,

                    CategoryId = variant.Product.CategoryId,
                    CategoryName = variant.Product.Category != null
                        ? variant.Product.Category.Name
                        : null,

                    CountryId = variant.Product.CountryId,
                    CountryName = variant.Product.Country != null
                        ? variant.Product.Country.Name
                        : null,

                    ShortDescription = variant.Product.ShortDescription,
                    LocationText = variant.Product.LocationText,
                    ThumbnailUrl = variant.Product.ThumbnailUrl,

                    OriginalPrice = price.OriginalPrice,
                    SalePrice = price.SalePrice,
                    Currency = price.Currency,

                    IsFeatured = variant.Product.IsFeatured,
                    IsHot = variant.Product.IsHot,
                    SoldCount = variant.Product.SoldCount,

                    ProductSortOrder = variant.Product.SortOrder,
                    VariantSortOrder = variant.SortOrder,

                    Features = new List<ProductVariantFeaturePublicDto>()
                };

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.ProductSortOrder)
                .ThenBy(x => x.ProductName)
                .ThenBy(x => x.VariantSortOrder)
                .ThenBy(x => x.SalePrice)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            if (items.Count > 0)
            {
                var variantIds = items
                    .Select(x => x.ProductVariantId)
                    .Distinct()
                    .ToList();

                var features = await productVariantFeatures
                    .Where(x =>
                        variantIds.Contains(x.ProductVariantId) &&
                        x.IsActive)
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.Text)
                    .Select(x => new ProductVariantFeaturePublicDto
                    {
                        // Id = x.Id,
                        ProductVariantId = x.ProductVariantId,
                        Text = x.Text,
                        Icon = x.Icon,
                        SortOrder = x.SortOrder
                    })
                    .ToListAsync(cancellationToken);

                var featureLookup = features
                    .GroupBy(x => x.ProductVariantId)
                    .ToDictionary(
                        x => x.Key,
                        x => x.ToList());

                foreach (var item in items)
                {
                    if (featureLookup.TryGetValue(item.ProductVariantId, out var variantFeatures))
                    {
                        item.Features = variantFeatures;
                    }
                }
            }

            return new PagedResultDto<ProductVariantPublicDto>
            {
                Items = items,
                TotalCount = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<List<HomeEsimProductDto>> GetHomeEsimProductsAsync(
        CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            var rows = await _context.Products
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    !x.IsDeleted &&
                    x.CountryId != null &&
                    (x.IsFeatured || x.IsHot))
                .SelectMany(
                    product => _context.ProductPrices
                        .Where(price =>
                            price.ProductId == product.Id &&
                            price.IsActive &&
                            price.SalePrice > 0 &&
                            (price.StartDate == null || price.StartDate <= now) &&
                            (price.EndDate == null || price.EndDate >= now)),
                    (product, price) => new
                    {
                        CountryId = product.Country!.Id,
                        CountryName = product.Country.Name,
                        CountrySlug = product.Country.Slug,
                        FlagUrl = product.Country.FlagUrl,
                        Region = product.Country.Region ?? "Khu vực khác",
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ProductSlug = product.Slug,
                        LocationText = product.LocationText,
                        ThumbnailUrl = product.ThumbnailUrl,

                        IsHot = product.IsHot,
                        IsFeatured = product.IsFeatured,
                        SortOrder = product.SortOrder,

                        PriceFrom = price.SalePrice,
                        Currency = price.Currency
                    })
                .ToListAsync(cancellationToken);

            var items = rows
                .GroupBy(x => new
                {
                    x.CountryId,
                    x.CountryName,
                    x.Region,
                    x.CountrySlug,
                    x.FlagUrl
                })
                .Select(g =>
                {
                    var best = g
                        .OrderBy(x => x.PriceFrom)
                        .ThenByDescending(x => x.IsHot)
                        .ThenBy(x => x.SortOrder)
                        .ThenBy(x => x.ProductName)
                        .First();

                    return new HomeEsimProductDto
                    {
                        Id = best.ProductId,
                        Name = best.ProductName,
                        Slug = best.ProductSlug,
                        LocationText = best.LocationText,
                        ThumbnailUrl = best.ThumbnailUrl,

                        CountryId = best.CountryId,
                        CountryName = best.CountryName,
                        Region = best.Region,
                        CountrySlug = best.CountrySlug,
                        FlagUrl = best.FlagUrl,

                        IsHot = best.IsHot,
                        IsFeatured = best.IsFeatured,

                        PriceFrom = best.PriceFrom,
                        Currency = best.Currency
                    };
                })
                .OrderBy(x => x.CountryName)
                .ToList();

            return items;
        }

        //public async Task<List<HomeEsimProductDto>> GetHomeEsimProductsAsync(
        // CancellationToken cancellationToken = default)
        //{
        //    var now = DateTime.UtcNow;

        //    var items = await _context.Products
        //        .AsNoTracking()
        //        .Where(x => x.IsActive && !x.IsDeleted)
        //        .Where(x => x.IsFeatured || x.IsHot)
        //        .OrderByDescending(x => x.IsHot)
        //        .ThenBy(x => x.SortOrder)
        //        .ThenBy(x => x.Name)
        //        .Select(x => new HomeEsimProductDto
        //        {
        //            Id = x.Id,
        //            Name = x.Name,
        //            Slug = x.Slug,
        //            LocationText = x.LocationText,
        //            ThumbnailUrl = x.ThumbnailUrl,

        //            FlagUrl = x.Country != null
        //                ? x.Country.FlagUrl
        //                : null,

        //            IsHot = x.IsHot,
        //            IsFeatured = x.IsFeatured,

        //            PriceFrom = _context.ProductPrices
        //                .Where(p =>
        //                    p.ProductId == x.Id &&
        //                    p.IsActive &&
        //                    p.SalePrice > 0 &&
        //                    (p.StartDate == null || p.StartDate <= now) &&
        //                    (p.EndDate == null || p.EndDate >= now))
        //                .OrderBy(p => p.SalePrice)
        //                .Select(p => p.SalePrice)
        //                .FirstOrDefault(),

        //            Currency = _context.ProductPrices
        //                .Where(p =>
        //                    p.ProductId == x.Id &&
        //                    p.IsActive &&
        //                    p.SalePrice > 0 &&
        //                    (p.StartDate == null || p.StartDate <= now) &&
        //                    (p.EndDate == null || p.EndDate >= now))
        //                .OrderBy(p => p.SalePrice)
        //                .Select(p => p.Currency)
        //                .FirstOrDefault() ?? "VND"
        //        })
        //        .Where(x => x.PriceFrom > 0)
        //        .Take(10)
        //        .ToListAsync(cancellationToken);

        //    return items;
        //}
    }
}
