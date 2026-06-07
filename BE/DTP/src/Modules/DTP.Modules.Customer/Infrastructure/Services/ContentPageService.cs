using DTP.Modules.Content.Application.Abstractions;
using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Infrastructure.Services
{
    public class ContentPageService : IContentPageService
    {
        private readonly IContentPageRepository _repository;
        private readonly IContentUnitOfWork _unitOfWork;

        public ContentPageService(
            IContentPageRepository repository,
            IContentUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ContentPageDto> CreateAsync(
            string code,
            string title,
            string slug,
            string? summary,
            string content,
            ContentPageStatus status,
            int sortOrder,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new Exception("Page code is required.");

            if (string.IsNullOrWhiteSpace(title))
                throw new Exception("Page title is required.");

            if (string.IsNullOrWhiteSpace(slug))
                throw new Exception("Page slug is required.");

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("Page content is required.");

            if (await _repository.ExistsCodeAsync(code, null, cancellationToken))
                throw new Exception("Page code already exists.");

            if (await _repository.ExistsSlugAsync(slug, null, cancellationToken))
                throw new Exception("Page slug already exists.");

            var page = new ContentPage(
                code,
                title,
                slug,
                summary,
                content,
                status,
                sortOrder);

            await _repository.AddAsync(page, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(page);
        }

        public async Task<ContentPageDto> UpdateAsync(
            Guid id,
            string title,
            string slug,
            string? summary,
            string content,
            string? thumbnailUrl,
            ContentPageStatus status,
            int sortOrder,
            CancellationToken cancellationToken = default)
        {
            var page = await _repository.GetByIdAsync(id, cancellationToken);

            if (page == null)
                throw new Exception("Content page not found.");

            if (string.IsNullOrWhiteSpace(title))
                throw new Exception("Page title is required.");

            if (string.IsNullOrWhiteSpace(slug))
                throw new Exception("Page slug is required.");

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("Page content is required.");

            if (await _repository.ExistsSlugAsync(slug, id, cancellationToken))
                throw new Exception("Page slug already exists.");

            page.Update(
                title,
                slug,
                summary,
                content,
                thumbnailUrl,
                status,
                sortOrder);

            _repository.Update(page);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(page);
        }

        public async Task<bool> HideAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var page = await _repository.GetByIdAsync(id, cancellationToken);

            if (page == null)
                throw new Exception("Content page not found.");

            page.Hide();

            _repository.Update(page);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<ContentPageDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var page = await _repository.GetByIdAsync(id, cancellationToken);
            return page == null ? null : Map(page);
        }

        public async Task<ContentPageDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            var page = await _repository.GetBySlugAsync(slug, cancellationToken);

            if (page == null || !page.IsPublished)
                return null;

            return Map(page);
        }

        public async Task<PagedResultDto<ContentPageDto>> GetPagedAsync(
            string? keyword,
            ContentPageStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 20;

            var result = await _repository.GetPagedAsync(
                keyword,
                status,
                pageIndex,
                pageSize,
                cancellationToken);

            return new PagedResultDto<ContentPageDto>(
                result.Items.Select(Map).ToList(),
                result.TotalCount,
                pageIndex,
                pageSize);
        }

        public async Task<IReadOnlyList<ContentPageDto>> GetPublishedAsync(CancellationToken cancellationToken = default)
        {
            var pages = await _repository.GetPublishedAsync(cancellationToken);
            return pages.Select(Map).ToList();
        }

        private static ContentPageDto Map(ContentPage page)
        {
            return new ContentPageDto
            {
                Id = page.Id,
                Code = page.Code,
                Title = page.Title,
                Slug = page.Slug,
                Summary = page.Summary,
                Content = page.Content,
                ThumbnailUrl = page.ThumbnailUrl,
                Status = page.Status,
                SortOrder = page.SortOrder,
                CreatedAt = page.CreatedAt,
                PublishedAt = page.PublishedAt
            };
        }
    }
}
