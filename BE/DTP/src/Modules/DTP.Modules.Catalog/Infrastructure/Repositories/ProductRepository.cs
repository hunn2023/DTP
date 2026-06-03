using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Commands.Products;
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

        public async Task<PagedResultDto<ProductDto>> GetPublicPagedAsync(
            string? keyword,
            Guid? categoryId,
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
                command.Code,
                command.Name,
                command.Slug,
                command.CategoryId,
                command.ShortDescription,
                command.Description,
                command.ThumbnailUrl,
                command.SortOrder);

            if (!command.IsActive)
            {
                product.Update(
                    command.Code,
                    command.Name,
                    command.Slug,
                    command.CategoryId,
                    command.ShortDescription,
                    command.Description,
                    command.ThumbnailUrl,
                    command.SortOrder,
                    false);
            }

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
            {
                return false;
            }

            product.Update(
                command.Code,
                command.Name,
                command.Slug,
                command.CategoryId,
                command.ShortDescription,
                command.Description,
                command.ThumbnailUrl,
                command.SortOrder,
                command.IsActive);

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
            bool? isActive)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Name.Contains(keyword) ||
                    x.Slug.Contains(keyword) ||
                    (x.Code != null && x.Code.Contains(keyword)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x =>
                    x.CategoryId == categoryId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(x =>
                    x.IsActive == isActive.Value);
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
                    //CategoryName = x.Category.Name,
                    ShortDescription = x.ShortDescription,
                    Description = x.Description,
                    ThumbnailUrl = x.ThumbnailUrl,
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
