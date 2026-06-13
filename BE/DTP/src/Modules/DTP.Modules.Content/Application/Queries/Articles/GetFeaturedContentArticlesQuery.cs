using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Queries.Articles
{
    public record GetFeaturedContentArticlesQuery(
      int Take) : IRequest<Result<IReadOnlyList<ContentArticleListItemDto>>>;


    public class GetFeaturedContentArticlesQueryHandler
    : IRequestHandler<GetFeaturedContentArticlesQuery, Result<IReadOnlyList<ContentArticleListItemDto>>>
    {
        private readonly IContentArticleService _service;

        public GetFeaturedContentArticlesQueryHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<Result<IReadOnlyList<ContentArticleListItemDto>>> Handle(
            GetFeaturedContentArticlesQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetFeaturedAsync(
                request.Take,
                cancellationToken);
        }
    }
}
