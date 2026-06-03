using DTP.Modules.Catalog.Application.Commands.PhoneCards;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Persistence;
using System;


namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface IPhoneCardRepository : IRepositoryBase<PhoneCard>
    {
        Task<PagedResultDto<PhoneCardDto>> GetPublicPagedAsync(
         Guid? providerId,
         decimal? minFaceValue,
         decimal? maxFaceValue,
         int pageIndex,
         int pageSize,
         CancellationToken cancellationToken = default);

        Task<PhoneCardDto?> GetPublicBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<PhoneCardDto>> GetPagedAsync(
            string? keyword,
            Guid? productVariantId,
            Guid? providerId,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<PhoneCardDto?> GetByIdDtoAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(
            CreatePhoneCardCommand command,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            UpdatePhoneCardCommand command,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
