using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using MediatR;

namespace DTP.Modules.Content.Application.Queries.Articles
{
    public record GetContentArticleBySlugQuery(
      string Slug,
      bool IncreaseView) : IRequest<ContentArticleDto?>;


    public class GetContentArticleBySlugQueryHandler
    : IRequestHandler<GetContentArticleBySlugQuery, ContentArticleDto?>
    {
        private readonly IContentArticleService _service;

        public GetContentArticleBySlugQueryHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<ContentArticleDto?> Handle(
            GetContentArticleBySlugQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetBySlugAsync(
                request.Slug,
                request.IncreaseView,
                cancellationToken);
        }
    }
}
