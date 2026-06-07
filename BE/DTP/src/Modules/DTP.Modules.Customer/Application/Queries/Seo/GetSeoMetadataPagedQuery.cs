using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Queries.Seo
{
    public record GetSeoMetadataPagedQuery(
      string? Keyword,
      string? EntityType,
      int PageIndex,
      int PageSize) : IRequest<PagedResultDto<SeoMetadataDto>>;


    public class GetSeoMetadataPagedQueryHandler
    : IRequestHandler<GetSeoMetadataPagedQuery, PagedResultDto<SeoMetadataDto>>
    {
        private readonly ISeoMetadataService _service;

        public GetSeoMetadataPagedQueryHandler(ISeoMetadataService service)
        {
            _service = service;
        }

        public Task<PagedResultDto<SeoMetadataDto>> Handle(
            GetSeoMetadataPagedQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetPagedAsync(
                request.Keyword,
                request.EntityType,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
