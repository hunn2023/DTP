using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Articles
{
    public record MarkContentArticleFeaturedCommand(Guid Id) : IRequest<Result>;

    public class MarkContentArticleFeaturedCommandHandler
    : IRequestHandler<MarkContentArticleFeaturedCommand, Result>
    {
        private readonly IContentArticleService _service;

        public MarkContentArticleFeaturedCommandHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<Result> Handle(
            MarkContentArticleFeaturedCommand request,
            CancellationToken cancellationToken)
        {
            return _service.MarkAsFeaturedAsync(request.Id, cancellationToken);
        }
    }
}
