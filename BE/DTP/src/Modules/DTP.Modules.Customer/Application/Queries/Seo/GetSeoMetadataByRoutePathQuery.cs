using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Queries.Seo
{
    public record GetSeoMetadataByRoutePathQuery(
     string RoutePath) : IRequest<SeoMetadataDto?>;

    public class GetSeoMetadataByRoutePathQueryHandler
    : IRequestHandler<GetSeoMetadataByRoutePathQuery, SeoMetadataDto?>
    {
        private readonly ISeoMetadataService _service;

        public GetSeoMetadataByRoutePathQueryHandler(ISeoMetadataService service)
        {
            _service = service;
        }

        public Task<SeoMetadataDto?> Handle(
            GetSeoMetadataByRoutePathQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetByRoutePathAsync(
                request.RoutePath,
                cancellationToken);
        }
    }
}
