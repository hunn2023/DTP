using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Queries.Seo
{
    public record GetSeoMetadataByEntityQuery(
      string EntityType,
      Guid EntityId) : IRequest<SeoMetadataDto?>;


    public class GetSeoMetadataByEntityQueryHandler
    : IRequestHandler<GetSeoMetadataByEntityQuery, SeoMetadataDto?>
    {
        private readonly ISeoMetadataService _service;

        public GetSeoMetadataByEntityQueryHandler(ISeoMetadataService service)
        {
            _service = service;
        }

        public Task<SeoMetadataDto?> Handle(
            GetSeoMetadataByEntityQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetByEntityAsync(
                request.EntityType,
                request.EntityId,
                cancellationToken);
        }
    }
}
