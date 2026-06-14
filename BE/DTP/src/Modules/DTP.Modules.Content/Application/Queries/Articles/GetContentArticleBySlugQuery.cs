using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Queries.Articles
{
    public record GetContentArticleBySlugQuery(
      string Slug,
      bool IncreaseView) : IRequest<Result<ContentArticleDto?>>;


    public class GetContentArticleBySlugQueryHandler
    : IRequestHandler<GetContentArticleBySlugQuery, Result<ContentArticleDto?>>
    {
        private readonly IContentArticleService _service;

        public GetContentArticleBySlugQueryHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<Result<ContentArticleDto?>> Handle(
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
