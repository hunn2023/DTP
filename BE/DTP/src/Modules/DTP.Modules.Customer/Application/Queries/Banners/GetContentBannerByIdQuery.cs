using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Queries.Banners
{
    public record GetContentBannerByIdQuery(Guid Id) : IRequest<ContentBannerDto?>;

    public class GetContentBannerByIdQueryHandler
    : IRequestHandler<GetContentBannerByIdQuery, ContentBannerDto?>
    {
        private readonly IContentBannerService _service;

        public GetContentBannerByIdQueryHandler(IContentBannerService service)
        {
            _service = service;
        }

        public Task<ContentBannerDto?> Handle(
            GetContentBannerByIdQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
