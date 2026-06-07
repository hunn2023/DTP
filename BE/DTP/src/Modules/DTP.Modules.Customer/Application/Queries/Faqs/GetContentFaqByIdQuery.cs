using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Queries.Faqs
{
    public record GetContentFaqByIdQuery(Guid Id) : IRequest<ContentFaqDto?>;


    public class GetContentFaqByIdQueryHandler
    : IRequestHandler<GetContentFaqByIdQuery, ContentFaqDto?>
    {
        private readonly IContentFaqService _service;

        public GetContentFaqByIdQueryHandler(IContentFaqService service)
        {
            _service = service;
        }

        public Task<ContentFaqDto?> Handle(
            GetContentFaqByIdQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
