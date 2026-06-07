using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Queries.Seo
{
    public record GetSeoMetadataByIdQuery(Guid Id) : IRequest<SeoMetadataDto?>;


    public class GetSeoMetadataByIdQueryHandler
    : IRequestHandler<GetSeoMetadataByIdQuery, SeoMetadataDto?>
    {
        private readonly ISeoMetadataService _service;

        public GetSeoMetadataByIdQueryHandler(ISeoMetadataService service)
        {
            _service = service;
        }

        public Task<SeoMetadataDto?> Handle(
            GetSeoMetadataByIdQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetByIdAsync(
                request.Id,
                cancellationToken);
        }
    }
}
