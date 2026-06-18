using DTP.Modules.Content.Application.Abstractions;
using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Knowledge.Application.Commands.ReindexKnowledge;
using DTP.Modules.Knowledge.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;
using MediatR;

namespace DTP.Modules.Content.Infrastructure.Services
{
    public class ContentFaqService : IContentFaqService
    {
        private readonly IContentFaqRepository _repository;
        private readonly IContentUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IMediator _mediator;
        public ContentFaqService(
            IContentFaqRepository repository,
            IContentUnitOfWork unitOfWork,
            ICacheService cacheService,
             IMediator mediator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _mediator = mediator;
        }

        public async Task<Result<ContentFaqDto>> CreateAsync(
                   string question,
                   string answer,
                   string? categoryCode,
                   int sortOrder,
                   bool isActive,
                   CancellationToken cancellationToken = default)
        {
            ValidateFaq(question, answer);

            categoryCode = NormalizeCategoryCode(categoryCode);

            var faq = new ContentFaq(
                question,
                answer,
                categoryCode,
                sortOrder,
                isActive);

            await _repository.AddAsync(faq, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearRelatedCacheAsync(cancellationToken);

            return Result<ContentFaqDto>.Success(Map(faq));
        }

        public async Task<Result<ContentFaqDto>> UpdateAsync(
            Guid id,
            string question,
            string answer,
            string? categoryCode,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var faq = await _repository.GetByIdAsync(id, cancellationToken);

            if (faq == null) return Result<ContentFaqDto>.Failure("FAQ not found.");

            ValidateFaq(question, answer);

            categoryCode = NormalizeCategoryCode(categoryCode);

            faq.Update(
                question,
                answer,
                categoryCode,
                sortOrder,
                isActive);

            _repository.Update(faq);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearRelatedCacheAsync(cancellationToken);

            await _mediator.Send(
                 new ReindexKnowledgeSourceCommand(
                     KnowledgeSourceType.ContentFaq,
                     faq.Id),
                 cancellationToken);

            return Result<ContentFaqDto>.Success(Map(faq));

        }

        public async Task<Result> EnableAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var faq = await _repository.GetByIdAsync(id, cancellationToken);

            if (faq == null)
                return Result.Failure("FAQ not found.");

            faq.Enable();


            _repository.Update(faq);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearRelatedCacheAsync(cancellationToken);

            await _mediator.Send(
             new ReindexKnowledgeSourceCommand(
                 KnowledgeSourceType.ContentFaq,
                 faq.Id),
             cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DisableAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var faq = await _repository.GetByIdAsync(id, cancellationToken);

            if (faq == null)
                throw new Exception("FAQ not found.");

            faq.Disable();

            _repository.Update(faq);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearRelatedCacheAsync(cancellationToken);

            await _mediator.Send(
            new ReindexKnowledgeSourceCommand(
                KnowledgeSourceType.ContentFaq,
                faq.Id),
            cancellationToken);
            return Result.Success();
        }

        public async Task<Result<ContentFaqDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result<ContentFaqDto?>.Failure("Invalid ID.");

            var cacheKey = CacheKeys.Detail(id);

            var cached = await _cacheService.GetAsync<ContentFaqDto?>(
                cacheKey,
                cancellationToken);

            if (cached != null)
                return Result<ContentFaqDto?>.Success(cached);

            var faq = await _repository.GetByIdAsync(id, cancellationToken);

            if (faq == null)
                return Result<ContentFaqDto?>.Failure("FAQ not found.");

            var dto = Map(faq);

            await _cacheService.SetAsync(
                cacheKey,
                dto,
                TimeSpan.FromMinutes(10),
                cancellationToken);

            return Result<ContentFaqDto?>.Success(dto);
        }

        public async Task<Result<PagedResultDto<ContentFaqDto>>> GetPagedAsync(
            string? keyword,
            string? categoryCode,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            NormalizePaging(ref pageIndex, ref pageSize);

            categoryCode = NormalizeCategoryCode(categoryCode);

            var cacheKey = CacheKeys.Paged(
                keyword,
                categoryCode,
                isActive,
                pageIndex,
                pageSize);

            var cached = await _cacheService.GetAsync<PagedResultDto<ContentFaqDto>>(
                cacheKey,
                cancellationToken);

            if (cached != null)
                return Result<PagedResultDto<ContentFaqDto>>.Success(cached);


            var result = await _repository.GetPagedAsync(
                keyword,
                categoryCode,
                isActive,
                pageIndex,
                pageSize,
                cancellationToken);

            var dto = new PagedResultDto<ContentFaqDto>(
                result.Items.Select(Map).ToList(),
                result.TotalCount,
                pageIndex,
                pageSize);

            await _cacheService.SetAsync(
                cacheKey,
                dto,
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return Result<PagedResultDto<ContentFaqDto>>.Success(dto);
        }

        public async Task<Result<IReadOnlyList<ContentFaqDto>>> GetActiveAsync(
            string? categoryCode,
            CancellationToken cancellationToken = default)
        {
            categoryCode = NormalizeCategoryCode(categoryCode);

            var cacheKey = CacheKeys.Active(categoryCode);

            var cached = await _cacheService.GetAsync<IReadOnlyList<ContentFaqDto>>(
                cacheKey,
                cancellationToken);

            if (cached != null) return Result<IReadOnlyList<ContentFaqDto>>.Success(cached);

            var faqs = await _repository.GetActiveAsync(
                categoryCode,
                cancellationToken);

            var dto = faqs.Select(Map).ToList();

            await _cacheService.SetAsync(
                cacheKey,
                dto,
                TimeSpan.FromMinutes(10),
                cancellationToken);

            return Result<IReadOnlyList<ContentFaqDto>>.Success(dto);

        }

        private async Task ClearRelatedCacheAsync(
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveByPrefixAsync(
                CacheKeys.Prefix,
                cancellationToken);
        }

        private static void NormalizePaging(
            ref int pageIndex,
            ref int pageSize)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 20;

            if (pageSize > 100)
                pageSize = 100;
        }

        private static void ValidateFaq(
            string question,
            string answer)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new Exception("FAQ question is required.");

            if (string.IsNullOrWhiteSpace(answer))
                throw new Exception("FAQ answer is required.");

            if (question.Trim().Length > 1000)
                throw new Exception("FAQ question cannot exceed 1000 characters.");
        }

        private static string? NormalizeCategoryCode(string? categoryCode)
        {
            return string.IsNullOrWhiteSpace(categoryCode)
                ? null
                : categoryCode.Trim().ToUpperInvariant();
        }

        private static ContentFaqDto Map(ContentFaq faq)
        {
            return new ContentFaqDto
            {
                Id = faq.Id,
                Question = faq.Question,
                Answer = faq.Answer,
                CategoryCode = faq.CategoryCode,
                SortOrder = faq.SortOrder,
                IsActive = faq.IsActive
            };
        }

        private static class CacheKeys
        {
            public const string Prefix = "content:faq";

            public static string Detail(Guid id)
                => $"{Prefix}:detail:{id}";

            public static string Paged(
                string? keyword,
                string? categoryCode,
                bool? isActive,
                int pageIndex,
                int pageSize)
                => $"{Prefix}:paged:{Normalize(keyword)}:{Normalize(categoryCode)}:{NormalizeBool(isActive)}:{pageIndex}:{pageSize}";

            public static string Active(string? categoryCode)
                => $"{Prefix}:active:{Normalize(categoryCode)}";

            private static string Normalize(string? value)
                => string.IsNullOrWhiteSpace(value)
                    ? "all"
                    : value.Trim().ToLowerInvariant();

            private static string NormalizeBool(bool? value)
                => value.HasValue
                    ? value.Value.ToString().ToLowerInvariant()
                    : "all";
        }
    }
}
