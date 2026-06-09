using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Content.Application.Queries.Banners
{
    public record GetContentBannersPagedQuery(
        string? Keyword,
        ContentBannerPosition? Position,
        bool? IsActive,
        int PageIndex,
        int PageSize) : IRequest<PagedResultDto<ContentBannerDto>>;


    public class GetContentBannersPagedQueryHandler
    : IRequestHandler<GetContentBannersPagedQuery, PagedResultDto<ContentBannerDto>>
    {
        private readonly IContentBannerService _service;

        public GetContentBannersPagedQueryHandler(IContentBannerService service)
        {
            _service = service;
        }

        public Task<PagedResultDto<ContentBannerDto>> Handle(
            GetContentBannersPagedQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetPagedAsync(
                request.Keyword,
                request.Position,
                request.IsActive,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
