using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Content.Application.Queries.Articles
{
    public record GetContentArticlesPagedQuery(
       string? Keyword,
       string? CategoryCode,
       ContentArticleStatus? Status,
       bool? IsFeatured,
       int PageIndex,
       int PageSize) : IRequest<PagedResultDto<ContentArticleListItemDto>>;


    public class GetContentArticlesPagedQueryHandler
    : IRequestHandler<GetContentArticlesPagedQuery, PagedResultDto<ContentArticleListItemDto>>
    {
        private readonly IContentArticleService _service;

        public GetContentArticlesPagedQueryHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<PagedResultDto<ContentArticleListItemDto>> Handle(
            GetContentArticlesPagedQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetPagedAsync(
                request.Keyword,
                request.CategoryCode,
                request.Status,
                request.IsFeatured,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
