using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Knowledge.Application.Commands.ReindexKnowledge;
using DTP.Modules.Knowledge.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Caching;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProductFaqService : IProductFaqService
    {
        private readonly IProductFaqRepository _productFaqRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IMediator _mediator;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public ProductFaqService(
            IProductFaqRepository productFaqRepository,
            IProductRepository productRepository,
            ICatalogUnitOfWork unitOfWork,
            ICacheService cacheService,
            IMediator mediator)
        {
            _productFaqRepository = productFaqRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _mediator = mediator;
        }

        public async Task<Result<ProductFaqDto>> CreateAsync(
            CreateProductFaqDto request,
            CancellationToken cancellationToken = default)
        {
            ValidateCreate(request);

            var productExists = await _productRepository.ExistsAsync(
                request.ProductId,
                cancellationToken);

            if (!productExists)
            {
                return Result<ProductFaqDto>.Failure("Sản phẩm không tồn tại.");
            }

            var productFaq = new ProductFaq(
                request.ProductId,
                request.Question,
                request.Answer,
                request.SortOrder,
                request.IsActive);

            await _productFaqRepository.AddAsync(productFaq, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearProductFaqCacheAsync(cancellationToken);


            return Result<ProductFaqDto>.Success(MapToDto(productFaq));
        }

        public async Task<Result<ProductFaqDto>> UpdateAsync(
            Guid id,
            UpdateProductFaqDto request,
            CancellationToken cancellationToken = default)
        {
            ValidateUpdate(request);

            if (id == Guid.Empty)
            {
                return Result<ProductFaqDto>.Failure("Id FAQ sản phẩm không hợp lệ.");
            }

            var productFaq = await _productFaqRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (productFaq is null)
            {
                return Result<ProductFaqDto>.Failure("FAQ sản phẩm không tồn tại.");
            }

            productFaq.Update(
                request.Question,
                request.Answer,
                request.SortOrder,
                request.IsActive);

            _productFaqRepository.Update(productFaq);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearProductFaqCacheAsync(cancellationToken);


            await _mediator.Send(
                new ReindexKnowledgeSourceCommand(
                    KnowledgeSourceType.ProductFaq,
                    productFaq.Id),
                cancellationToken);

            return Result<ProductFaqDto>.Success(MapToDto(productFaq));
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return Result.Failure("Id FAQ sản phẩm không hợp lệ.");
            }

            var productFaq = await _productFaqRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (productFaq is null)
            {
                return Result.Failure("FAQ sản phẩm không tồn tại.");
            }

            _productFaqRepository.Remove(productFaq);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearProductFaqCacheAsync(cancellationToken);

            await _mediator.Send(
                new ReindexKnowledgeSourceCommand(
                    KnowledgeSourceType.ProductFaq,
                    productFaq.Id),
                cancellationToken);

            return Result.Success();
        }

        public async Task<Result<ProductFaqDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return Result<ProductFaqDto?>.Failure("Id FAQ sản phẩm không hợp lệ.");
            }

            var cacheKey = ProductFaqCacheKeys.ById(id);

            var cachedData = await _cacheService.GetAsync<ProductFaqDto>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return Result<ProductFaqDto?>.Success(cachedData);

            }

            var productFaq = await _productFaqRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (productFaq is null)
            {
                return Result<ProductFaqDto?>.Failure("FAQ sản phẩm không tồn tại.");
            }

            var result = MapToDto(productFaq);

            await _cacheService.SetAsync(
                cacheKey,
                result,
                CacheDuration,
                cancellationToken);

            return Result<ProductFaqDto?>.Success(result);
        }

        public async Task<Result<List<ProductFaqDto>>> GetByProductIdAsync(
            Guid productId,
            bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
            {
                return Result<List<ProductFaqDto>>.Failure("ProductId không hợp lệ.");

            }

            var cacheKey = ProductFaqCacheKeys.ByProductId(
                productId,
                onlyActive);

            var cachedData = await _cacheService.GetAsync<List<ProductFaqDto>>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return Result<List<ProductFaqDto>>.Success(cachedData);
            }

            var productFaqs = await _productFaqRepository.GetByProductIdAsync(
                productId,
                onlyActive,
                cancellationToken);

            var result = productFaqs
                .OrderBy(x => x.SortOrder)
                .Select(MapToDto)
                .ToList();

            await _cacheService.SetAsync(
                cacheKey,
                result,
                CacheDuration,
                cancellationToken);

            return Result<List<ProductFaqDto>>.Success(result);

        }

        private async Task ClearProductFaqCacheAsync(
            CancellationToken cancellationToken)
        {
            await _cacheService.RemoveByPrefixAsync(
                ProductFaqCacheKeys.PrefixAll(),
                cancellationToken);
        }

        private static void ValidateCreate(CreateProductFaqDto request)
        {
            if (request.ProductId == Guid.Empty)
            {
                throw new ArgumentException("ProductId không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(request.Question))
            {
                throw new ArgumentException("Câu hỏi không được để trống.");
            }

            if (request.Question.Length > 500)
            {
                throw new ArgumentException("Câu hỏi không được vượt quá 500 ký tự.");
            }

            if (string.IsNullOrWhiteSpace(request.Answer))
            {
                throw new ArgumentException("Câu trả lời không được để trống.");
            }
        }

        private static void ValidateUpdate(UpdateProductFaqDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                throw new ArgumentException("Câu hỏi không được để trống.");
            }

            if (request.Question.Length > 500)
            {
                throw new ArgumentException("Câu hỏi không được vượt quá 500 ký tự.");
            }

            if (string.IsNullOrWhiteSpace(request.Answer))
            {
                throw new ArgumentException("Câu trả lời không được để trống.");
            }
        }

        private static ProductFaqDto MapToDto(ProductFaq productFaq)
        {
            return new ProductFaqDto
            {
                Id = productFaq.Id,
                ProductId = productFaq.ProductId,
                Question = productFaq.Question,
                Answer = productFaq.Answer,
                SortOrder = productFaq.SortOrder,
                IsActive = productFaq.IsActive,
                CreatedAt = productFaq.CreatedAt,
                UpdatedAt = productFaq.UpdatedAt
            };
        }
    }
}
