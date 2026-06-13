using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;
namespace DTP.Modules.Content.Application.Queries.Articles
{
    public record GetPublicContentArticlesPagedQuery(
      string? Keyword,
      string? CategoryCode,
      int PageIndex,
      int PageSize) : IRequest<Result<PagedResultDto<ContentArticleListItemDto>>>;

    public class GetPublicContentArticlesPagedQueryHandler
    : IRequestHandler<GetPublicContentArticlesPagedQuery, Result<PagedResultDto<ContentArticleListItemDto>>>
    {
        private readonly IContentArticleService _service;

        public GetPublicContentArticlesPagedQueryHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<Result<PagedResultDto<ContentArticleListItemDto>>> Handle(
            GetPublicContentArticlesPagedQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetPublicPagedAsync(
                request.Keyword,
                request.CategoryCode,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
