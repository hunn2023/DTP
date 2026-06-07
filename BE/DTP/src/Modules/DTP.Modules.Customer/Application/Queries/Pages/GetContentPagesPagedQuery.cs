using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Queries.Pages
{

    public record GetContentPagesPagedQuery(
        string? Keyword,
        ContentPageStatus? Status,
        int PageIndex,
        int PageSize) : IRequest<PagedResultDto<ContentPageDto>>;

    public class GetContentPagesPagedQueryHandler
    : IRequestHandler<GetContentPagesPagedQuery, PagedResultDto<ContentPageDto>>
    {
        private readonly IContentPageService _service;

        public GetContentPagesPagedQueryHandler(IContentPageService service)
        {
            _service = service;
        }

        public Task<PagedResultDto<ContentPageDto>> Handle(
            GetContentPagesPagedQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetPagedAsync(
                request.Keyword,
                request.Status,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
