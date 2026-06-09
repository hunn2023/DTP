using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using MediatR;

namespace DTP.Modules.Content.Application.Queries.Pages
{
    public record GetContentPageBySlugQuery(string Slug) : IRequest<ContentPageDto?>;

    public class GetContentPageBySlugQueryHandler
    : IRequestHandler<GetContentPageBySlugQuery, ContentPageDto?>
    {
        private readonly IContentPageService _service;

        public GetContentPageBySlugQueryHandler(IContentPageService service)
        {
            _service = service;
        }

        public Task<ContentPageDto?> Handle(
            GetContentPageBySlugQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetBySlugAsync(request.Slug, cancellationToken);
        }
    }
}
