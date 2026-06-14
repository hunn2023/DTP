using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Queries.Articles
{
    public record GetContentArticleByIdQuery(Guid Id) : IRequest<Result<ContentArticleDto?>>;

    public class GetContentArticleByIdQueryHandler
    : IRequestHandler<GetContentArticleByIdQuery, Result<ContentArticleDto?>>
    {
        private readonly IContentArticleService _service;

        public GetContentArticleByIdQueryHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<Result<ContentArticleDto?>> Handle(
            GetContentArticleByIdQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
