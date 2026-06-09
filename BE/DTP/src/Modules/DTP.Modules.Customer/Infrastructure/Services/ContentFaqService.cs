using DTP.Modules.Content.Application.Abstractions;
using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Entities;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Infrastructure.Services
{
    public class ContentFaqService : IContentFaqService
    {
        private readonly IContentFaqRepository _repository;
        private readonly IContentUnitOfWork _unitOfWork;

        public ContentFaqService(
            IContentFaqRepository repository,
            IContentUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ContentFaqDto> CreateAsync(
            string question,
            string answer,
            string? categoryCode,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            ValidateFaq(question, answer);

            var faq = new ContentFaq(
                question,
                answer,
                categoryCode,
                sortOrder,
                isActive);

            await _repository.AddAsync(faq, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(faq);
        }

        public async Task<ContentFaqDto> UpdateAsync(
            Guid id,
            string question,
            string answer,
            string? categoryCode,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var faq = await _repository.GetByIdAsync(id, cancellationToken);

            if (faq == null)
                throw new Exception("FAQ not found.");

            ValidateFaq(question, answer);

            faq.Update(
                question,
                answer,
                categoryCode,
                sortOrder,
                isActive);

            _repository.Update(faq);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(faq);
        }

        public async Task<bool> EnableAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var faq = await _repository.GetByIdAsync(id, cancellationToken);

            if (faq == null)
                throw new Exception("FAQ not found.");

            //faq.Enable();

            _repository.Update(faq);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DisableAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var faq = await _repository.GetByIdAsync(id, cancellationToken);

            if (faq == null)
                throw new Exception("FAQ not found.");

            faq.Disable();

            _repository.Update(faq);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<ContentFaqDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var faq = await _repository.GetByIdAsync(id, cancellationToken);

            return faq == null ? null : Map(faq);
        }

        public async Task<PagedResultDto<ContentFaqDto>> GetPagedAsync(
            string? keyword,
            string? categoryCode,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 20;

            categoryCode = NormalizeCategoryCode(categoryCode);

            var result = await _repository.GetPagedAsync(
                keyword,
                categoryCode,
                isActive,
                pageIndex,
                pageSize,
                cancellationToken);

            return new PagedResultDto<ContentFaqDto>(
                result.Items.Select(Map).ToList(),
                result.TotalCount,
                pageIndex,
                pageSize);
        }

        public async Task<IReadOnlyList<ContentFaqDto>> GetActiveAsync(
            string? categoryCode,
            CancellationToken cancellationToken = default)
        {
            categoryCode = NormalizeCategoryCode(categoryCode);

            var faqs = await _repository.GetActiveAsync(
                categoryCode,
                cancellationToken);

            return faqs.Select(Map).ToList();
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
    }
}
