using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Queries.Articles
{
    public record GetFeaturedContentArticlesQuery(
      int Take) : IRequest<IReadOnlyList<ContentArticleListItemDto>>;


    public class GetFeaturedContentArticlesQueryHandler
    : IRequestHandler<GetFeaturedContentArticlesQuery, IReadOnlyList<ContentArticleListItemDto>>
    {
        private readonly IContentArticleService _service;

        public GetFeaturedContentArticlesQueryHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<IReadOnlyList<ContentArticleListItemDto>> Handle(
            GetFeaturedContentArticlesQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetFeaturedAsync(
                request.Take,
                cancellationToken);
        }
    }
}
