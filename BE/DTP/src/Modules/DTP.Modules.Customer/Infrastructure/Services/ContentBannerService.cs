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
    public class ContentBannerService : IContentBannerService
    {
        private readonly IContentBannerRepository _repository;
        private readonly IContentUnitOfWork _unitOfWork;

        public ContentBannerService(
            IContentBannerRepository repository,
            IContentUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ContentBannerDto> CreateAsync(
            string title,
            string imageUrl,
            string? mobileImageUrl,
            string? linkUrl,
            string? description,
            ContentBannerPosition position,
            DateTime? startDate,
            DateTime? endDate,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            ValidateBanner(
                title,
                imageUrl,
                startDate,
                endDate);

            var banner = new ContentBanner(
                title,
                imageUrl,
                mobileImageUrl,
                linkUrl,
                description,
                position,
                startDate,
                endDate,
                sortOrder,
                isActive);

            await _repository.AddAsync(banner, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(banner);
        }

        public async Task<ContentBannerDto> UpdateAsync(
            Guid id,
            string title,
            string imageUrl,
            string? mobileImageUrl,
            string? linkUrl,
            string? description,
            ContentBannerPosition position,
            DateTime? startDate,
            DateTime? endDate,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var banner = await _repository.GetByIdAsync(id, cancellationToken);

            if (banner == null)
                throw new Exception("Content banner not found.");

            ValidateBanner(
                title,
                imageUrl,
                startDate,
                endDate);

            banner.Update(
                title,
                imageUrl,
                mobileImageUrl,
                linkUrl,
                description,
                position,
                startDate,
                endDate,
                sortOrder,
                isActive);

            _repository.Update(banner);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(banner);
        }

        public async Task<bool> DisableAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var banner = await _repository.GetByIdAsync(id, cancellationToken);

            if (banner == null)
                throw new Exception("Content banner not found.");

            banner.Disable();

            _repository.Update(banner);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> EnableAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var banner = await _repository.GetByIdAsync(id, cancellationToken);

            if (banner == null)
                throw new Exception("Content banner not found.");

            banner.Enable();

            _repository.Update(banner);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<ContentBannerDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var banner = await _repository.GetByIdAsync(id, cancellationToken);

            return banner == null ? null : Map(banner);
        }

        public async Task<PagedResultDto<ContentBannerDto>> GetPagedAsync(
            string? keyword,
            ContentBannerPosition? position,
            bool? isActive,
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
                position,
                isActive,
                pageIndex,
                pageSize,
                cancellationToken);

            return new PagedResultDto<ContentBannerDto>(
                result.Items.Select(Map).ToList(),
                result.TotalCount,
                pageIndex,
                pageSize);
        }

        public async Task<IReadOnlyList<ContentBannerDto>> GetAvailableAsync(
            ContentBannerPosition? position,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            var banners = await _repository.GetAvailableAsync(
                position,
                now,
                cancellationToken);

            return banners.Select(Map).ToList();
        }

        private static void ValidateBanner(
            string title,
            string imageUrl,
            DateTime? startDate,
            DateTime? endDate)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new Exception("Banner title is required.");

            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new Exception("Banner image url is required.");

            if (endDate.HasValue && startDate.HasValue && endDate.Value < startDate.Value)
                throw new Exception("EndDate must be greater than or equal to StartDate.");
        }

        private static ContentBannerDto Map(ContentBanner banner)
        {
            return new ContentBannerDto
            {
                Id = banner.Id,
                Title = banner.Title,
                ImageUrl = banner.ImageUrl,
                MobileImageUrl = banner.MobileImageUrl,
                LinkUrl = banner.LinkUrl,
                Description = banner.Description,
                Position = banner.Position,
                StartDate = banner.StartDate,
                EndDate = banner.EndDate,
                SortOrder = banner.SortOrder,
                IsActive = banner.IsActive,
                CreatedAt = banner.CreatedAt,
                UpdatedAt = banner.UpdatedAt
            };
        }
    }
