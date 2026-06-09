using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application.Pagination;
using MediatR;
namespace DTP.Modules.Content.Application.Queries.Articles
{
    public record GetPublicContentArticlesPagedQuery(
      string? Keyword,
      string? CategoryCode,
      int PageIndex,
      int PageSize) : IRequest<PagedResultDto<ContentArticleListItemDto>>;

    public class GetPublicContentArticlesPagedQueryHandler
    : IRequestHandler<GetPublicContentArticlesPagedQuery, PagedResultDto<ContentArticleListItemDto>>
    {
        private readonly IContentArticleService _service;

        public GetPublicContentArticlesPagedQueryHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<PagedResultDto<ContentArticleListItemDto>> Handle(
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
