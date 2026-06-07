using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Queries.Articles
{
    public record GetContentArticleByIdQuery(Guid Id) : IRequest<ContentArticleDto?>;

    public class GetContentArticleByIdQueryHandler
    : IRequestHandler<GetContentArticleByIdQuery, ContentArticleDto?>
    {
        private readonly IContentArticleService _service;

        public GetContentArticleByIdQueryHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<ContentArticleDto?> Handle(
            GetContentArticleByIdQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
