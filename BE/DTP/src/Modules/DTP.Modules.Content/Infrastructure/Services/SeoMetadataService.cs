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
    public class SeoMetadataService : ISeoMetadataService
    {
        private readonly ISeoMetadataRepository _repository;
        private readonly IContentUnitOfWork _unitOfWork;

        public SeoMetadataService(
            ISeoMetadataRepository repository,
            IContentUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SeoMetadataDto> CreateAsync(
            string entityType,
            Guid? entityId,
            string? routePath,
            string metaTitle,
            string? metaDescription,
            string? metaKeywords,
            string? canonicalUrl,
            string? ogTitle,
            string? ogDescription,
            string? ogImageUrl,
            string? robots,
            CancellationToken cancellationToken = default)
        {
            ValidateSeo(
                entityType,
                entityId,
                routePath,
                metaTitle,
                metaDescription);

            entityType = NormalizeEntityType(entityType);
            routePath = NormalizeRoutePath(routePath);

            if (entityId.HasValue)
            {
                var existsEntity = await _repository.ExistsEntityAsync(
                    entityType,
                    entityId.Value,
                    null,
                    cancellationToken);

                if (existsEntity)
                    throw new Exception("SEO metadata for this entity already exists.");
            }

            if (!string.IsNullOrWhiteSpace(routePath))
            {
                var existsRoute = await _repository.ExistsRoutePathAsync(
                    routePath,
                    null,
                    cancellationToken);

                if (existsRoute)
                    throw new Exception("SEO metadata for this route path already exists.");
            }

            var seo = new SeoMetadata(
                entityType,
                entityId,
                routePath,
                metaTitle,
                metaDescription,
                metaKeywords,
                canonicalUrl,
                ogTitle,
                ogDescription,
                ogImageUrl,
                robots);

            await _repository.AddAsync(seo, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(seo);
        }

        public async Task<SeoMetadataDto> UpdateAsync(
            Guid id,
            string entityType,
            Guid? entityId,
            string? routePath,
            string metaTitle,
            string? metaDescription,
            string? metaKeywords,
            string? canonicalUrl,
            string? ogTitle,
            string? ogDescription,
            string? ogImageUrl,
            string? robots,
            CancellationToken cancellationToken = default)
        {
            var seo = await _repository.GetByIdAsync(id, cancellationToken);

            if (seo == null)
                throw new Exception("SEO metadata not found.");

            ValidateSeo(
                entityType,
                entityId,
                routePath,
                metaTitle,
                metaDescription);

            entityType = NormalizeEntityType(entityType);
            routePath = NormalizeRoutePath(routePath);

            if (entityId.HasValue)
            {
                var existsEntity = await _repository.ExistsEntityAsync(
                    entityType,
                    entityId.Value,
                    id,
                    cancellationToken);

                if (existsEntity)
                    throw new Exception("SEO metadata for this entity already exists.");
            }

            if (!string.IsNullOrWhiteSpace(routePath))
            {
                var existsRoute = await _repository.ExistsRoutePathAsync(
                    routePath,
                    id,
                    cancellationToken);

                if (existsRoute)
                    throw new Exception("SEO metadata for this route path already exists.");
            }

            seo.UpdateTarget(
                entityType,
                entityId,
                routePath);

            seo.Update(
                metaTitle,
                metaDescription,
                metaKeywords,
                canonicalUrl,
                ogTitle,
                ogDescription,
                ogImageUrl,
                robots);

            _repository.Update(seo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(seo);
        }

        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var seo = await _repository.GetByIdAsync(id, cancellationToken);

            if (seo == null)
                throw new Exception("SEO metadata not found.");

            _repository.Delete(seo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<SeoMetadataDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var seo = await _repository.GetByIdAsync(id, cancellationToken);

            return seo == null ? null : Map(seo);
        }

        public async Task<SeoMetadataDto?> GetByEntityAsync(
            string entityType,
            Guid entityId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entityType))
                return null;

            entityType = NormalizeEntityType(entityType);

            var seo = await _repository.GetByEntityAsync(
                entityType,
                entityId,
                cancellationToken);

            return seo == null ? null : Map(seo);
        }

        public async Task<SeoMetadataDto?> GetByRoutePathAsync(
            string routePath,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(routePath))
                return null;

            routePath = NormalizeRoutePath(routePath)!;

            var seo = await _repository.GetByRoutePathAsync(
                routePath,
                cancellationToken);

            return seo == null ? null : Map(seo);
        }

        public async Task<PagedResultDto<SeoMetadataDto>> GetPagedAsync(
            string? keyword,
            string? entityType,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            NormalizePaging(ref pageIndex, ref pageSize);

            entityType = string.IsNullOrWhiteSpace(entityType)
                ? null
                : NormalizeEntityType(entityType);

            var result = await _repository.GetPagedAsync(
                keyword,
                entityType,
                pageIndex,
                pageSize,
                cancellationToken);

            return new PagedResultDto<SeoMetadataDto>(
                result.Items.Select(Map).ToList(),
                result.TotalCount,
                pageIndex,
                pageSize);
        }

        private static void ValidateSeo(
            string entityType,
            Guid? entityId,
            string? routePath,
            string metaTitle,
            string? metaDescription)
        {
            if (string.IsNullOrWhiteSpace(entityType))
                throw new Exception("EntityType is required.");

            if (!entityId.HasValue && string.IsNullOrWhiteSpace(routePath))
                throw new Exception("EntityId or RoutePath is required.");

            if (string.IsNullOrWhiteSpace(metaTitle))
                throw new Exception("Meta title is required.");

            if (metaTitle.Trim().Length > 255)
                throw new Exception("Meta title cannot exceed 255 characters.");

            if (!string.IsNullOrWhiteSpace(metaDescription) &&
                metaDescription.Trim().Length > 1000)
            {
                throw new Exception("Meta description cannot exceed 1000 characters.");
            }
        }

        private static string NormalizeEntityType(string entityType)
        {
            return entityType.Trim();
        }

        private static string? NormalizeRoutePath(string? routePath)
        {
            if (string.IsNullOrWhiteSpace(routePath))
                return null;

            routePath = routePath.Trim();

            if (!routePath.StartsWith("/"))
                routePath = "/" + routePath;

            return routePath.ToLowerInvariant();
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

        private static SeoMetadataDto Map(SeoMetadata seo)
        {
            return new SeoMetadataDto
            {
                Id = seo.Id,
                EntityType = seo.EntityType,
                EntityId = seo.EntityId,
                RoutePath = seo.RoutePath,
                MetaTitle = seo.MetaTitle,
                MetaDescription = seo.MetaDescription,
                MetaKeywords = seo.MetaKeywords,
                CanonicalUrl = seo.CanonicalUrl,
                OgTitle = seo.OgTitle,
                OgDescription = seo.OgDescription,
                OgImageUrl = seo.OgImageUrl,
                Robots = seo.Robots,
                CreatedAt = seo.CreatedAt,
                UpdatedAt = seo.UpdatedAt
            };
        }
    }
}
