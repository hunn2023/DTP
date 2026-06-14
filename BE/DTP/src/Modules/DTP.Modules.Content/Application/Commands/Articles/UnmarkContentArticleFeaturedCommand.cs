using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Articles
{
    public record UnmarkContentArticleFeaturedCommand(Guid Id) : IRequest<Result>;

    public class UnmarkContentArticleFeaturedCommandHandler
    : IRequestHandler<UnmarkContentArticleFeaturedCommand, Result>
    {
        private readonly IContentArticleService _service;

        public UnmarkContentArticleFeaturedCommandHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<Result> Handle(
            UnmarkContentArticleFeaturedCommand request,
            CancellationToken cancellationToken)
        {
            return _service.UnmarkAsFeaturedAsync(request.Id, cancellationToken);
        }
    }
}
