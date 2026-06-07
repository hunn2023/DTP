using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Queries.Banners
{
    public record GetAvailableContentBannersQuery(
      ContentBannerPosition? Position) : IRequest<IReadOnlyList<ContentBannerDto>>;


    public class GetAvailableContentBannersQueryHandler
    : IRequestHandler<GetAvailableContentBannersQuery, IReadOnlyList<ContentBannerDto>>
    {
        private readonly IContentBannerService _service;

        public GetAvailableContentBannersQueryHandler(IContentBannerService service)
        {
            _service = service;
        }

        public Task<IReadOnlyList<ContentBannerDto>> Handle(
            GetAvailableContentBannersQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetAvailableAsync(
                request.Position,
                cancellationToken);
        }
    }
}
