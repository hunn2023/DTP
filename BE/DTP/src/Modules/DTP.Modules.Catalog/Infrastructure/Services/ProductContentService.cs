using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Domain.Enums;
using DTP.Modules.Knowledge.Application.Commands.ReindexKnowledge;
using DTP.Modules.Knowledge.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Caching;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProductContentService : IProductContentService
    {
        private readonly IProductContentRepository _productContentRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IMediator _mediator;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public ProductContentService(
            IProductContentRepository productContentRepository,
            IProductRepository productRepository,
            ICatalogUnitOfWork unitOfWork,
            ICacheService cacheService,
            IMediator mediator)
        {
            _productContentRepository = productContentRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _mediator = mediator;
        }

        public async Task<Result<ProductContentDto>> CreateAsync(
            CreateProductContentDto request,
            CancellationToken cancellationToken = default)
        {
            ValidateCreate(request);

            var productExists = await _productRepository.ExistsAsync(
                request.ProductId,
                cancellationToken);

            if (!productExists)
            {
                return Result<ProductContentDto>.Failure("Sản phẩm không tồn tại.");
            }

            var productContent = new ProductContent(
                request.ProductId,
                request.ContentType,
                request.Title,
                request.Summary,
                request.BodyHtml,
                request.SortOrder,
                request.IsActive);

            await _productContentRepository.AddAsync(productContent, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearProductContentCacheAsync(
                productContent.Id,
                productContent.ProductId,
                cancellationToken);

            return Result<ProductContentDto>.Success(MapToDto(productContent));
        }

        public async Task<Result<ProductContentDto>> UpdateAsync(
            Guid id,
            UpdateProductContentDto request,
            CancellationToken cancellationToken = default)
        {
            ValidateUpdate(request);

            var productContent = await _productContentRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (productContent is null)
            {
                return Result<ProductContentDto>.Failure("Nội dung sản phẩm không tồn tại.");

            }

            var productId = productContent.ProductId;

            productContent.Update(
                request.ContentType,
                request.Title,
                request.Summary,
                request.BodyHtml,
                request.SortOrder,
                request.IsActive);

            _productContentRepository.Update(productContent);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearProductContentCacheAsync(
                productContent.Id,
                productContent.ProductId,
                cancellationToken);


            await _mediator.Send(
                new ReindexKnowledgeSourceCommand(
                    KnowledgeSourceType.Content,
                    productContent.Id),
                cancellationToken);

            return Result<ProductContentDto>.Success(MapToDto(productContent));
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var productContent = await _productContentRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (productContent is null)
            {
                return Result.Failure("Nội dung sản phẩm không tồn tại.");
            }

            var productId = productContent.ProductId;

            _productContentRepository.Remove(productContent);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearProductContentCacheAsync(
                 productContent.Id,
                 productContent.ProductId,
                 cancellationToken);

            return Result.Success();
        }

        public async Task<Result<ProductContentDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return Result<ProductContentDto?>.Failure("Id nội dung sản phẩm không hợp lệ.");
            }

            var cacheKey = ProductContentCacheKeys.ById(id);

            var cachedData = await _cacheService.GetAsync<ProductContentDto>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return Result<ProductContentDto?>.Success(cachedData);

            }

            var productContent = await _productContentRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (productContent is null)
            {
                return Result<ProductContentDto?>.Failure("Nội dung sản phẩm không tồn tại.");

            }

            var result = MapToDto(productContent);

            await _cacheService.SetAsync(
                cacheKey,
                result,
                CacheDuration,
                cancellationToken);

            return Result<ProductContentDto?>.Success(result);

        }

        public async Task<Result<List<ProductContentDto>>> GetByProductIdAsync(
            Guid productId,
            bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
            {
                return Result<List<ProductContentDto>>.Failure("ProductId không hợp lệ.");
            }

            var cacheKey = ProductContentCacheKeys.ByProductId(
                productId,
                onlyActive);

            var cachedData = await _cacheService.GetAsync<List<ProductContentDto>>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return Result<List<ProductContentDto>>.Success(cachedData);

            }

            var productContents = await _productContentRepository.GetByProductIdAsync(
                productId,
                onlyActive,
                cancellationToken);

            var result = productContents
                .OrderBy(x => x.SortOrder)
                .Select(MapToDto)
                .ToList();

            await _cacheService.SetAsync(
                cacheKey,
                result,
                CacheDuration,
                cancellationToken);

            return Result<List<ProductContentDto>>.Success(result);
        }

        public async Task<Result<List<ProductContentDto>>> GetByProductIdAndTypeAsync(
            Guid productId,
            ProductContentType contentType,
            bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
            {
                return Result<List<ProductContentDto>>.Failure("ProductId không hợp lệ.");
            }

            if (!Enum.IsDefined(typeof(ProductContentType), contentType))
            {
                return Result<List<ProductContentDto>>.Failure("Loại nội dung không hợp lệ.");
            }

            var cacheKey = ProductContentCacheKeys.ByProductIdAndType(
                productId,
                contentType,
                onlyActive);

            var cachedData = await _cacheService.GetAsync<List<ProductContentDto>>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return Result<List<ProductContentDto>>.Success(cachedData);

            }

            var productContents = await _productContentRepository.GetByProductIdAndTypeAsync(
                productId,
                contentType,
                onlyActive,
                cancellationToken);

            var result = productContents
                .OrderBy(x => x.SortOrder)
                .Select(MapToDto)
                .ToList();

            await _cacheService.SetAsync(
                cacheKey,
                result,
                CacheDuration,
                cancellationToken);

            return Result<List<ProductContentDto>>.Success(result);

        }

        private async Task ClearProductContentCacheAsync(
            Guid productContentId,
            Guid productId,
            CancellationToken cancellationToken)
        {
            await _cacheService.RemoveAsync(
                ProductContentCacheKeys.ById(productContentId),
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                ProductContentCacheKeys.PrefixByProductId(productId),
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                ProductContentCacheKeys.PrefixAll(),
                cancellationToken);
        }

        private static void ValidateCreate(CreateProductContentDto request)
        {
            if (request.ProductId == Guid.Empty)
            {
                throw new ArgumentException("ProductId không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Tiêu đề nội dung không được để trống.");
            }

            if (request.Title.Length > 250)
            {
                throw new ArgumentException("Tiêu đề nội dung không được vượt quá 250 ký tự.");
            }

            if (!string.IsNullOrWhiteSpace(request.Summary) && request.Summary.Length > 1000)
            {
                throw new ArgumentException("Tóm tắt nội dung không được vượt quá 1000 ký tự.");
            }

            if (string.IsNullOrWhiteSpace(request.BodyHtml))
            {
                throw new ArgumentException("Nội dung chi tiết không được để trống.");
            }

            if (!Enum.IsDefined(typeof(ProductContentType), request.ContentType))
            {
                throw new ArgumentException("Loại nội dung không hợp lệ.");
            }
        }

        private static void ValidateUpdate(UpdateProductContentDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Tiêu đề nội dung không được để trống.");
            }

            if (request.Title.Length > 250)
            {
                throw new ArgumentException("Tiêu đề nội dung không được vượt quá 250 ký tự.");
            }

            if (!string.IsNullOrWhiteSpace(request.Summary) && request.Summary.Length > 1000)
            {
                throw new ArgumentException("Tóm tắt nội dung không được vượt quá 1000 ký tự.");
            }

            if (string.IsNullOrWhiteSpace(request.BodyHtml))
            {
                throw new ArgumentException("Nội dung chi tiết không được để trống.");
            }

            if (!Enum.IsDefined(typeof(ProductContentType), request.ContentType))
            {
                throw new ArgumentException("Loại nội dung không hợp lệ.");
            }
        }

        private static ProductContentDto MapToDto(ProductContent productContent)
        {
            return new ProductContentDto
            {
                Id = productContent.Id,
                ProductId = productContent.ProductId,
                ContentType = productContent.ContentType,
                Title = productContent.Title,
                Summary = productContent.Summary,
                BodyHtml = productContent.BodyHtml,
                SortOrder = productContent.SortOrder,
                IsActive = productContent.IsActive,
                CreatedAt = productContent.CreatedAt,
                UpdatedAt = productContent.UpdatedAt
            };
        }
    }
}
