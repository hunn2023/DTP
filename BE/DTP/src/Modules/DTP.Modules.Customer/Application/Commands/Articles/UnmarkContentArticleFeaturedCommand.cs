using DTP.Modules.Content.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Articles
{
    public record UnmarkContentArticleFeaturedCommand(Guid Id) : IRequest<bool>;

    public class UnmarkContentArticleFeaturedCommandHandler
    : IRequestHandler<UnmarkContentArticleFeaturedCommand, bool>
    {
        private readonly IContentArticleService _service;

        public UnmarkContentArticleFeaturedCommandHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<bool> Handle(
            UnmarkContentArticleFeaturedCommand request,
            CancellationToken cancellationToken)
        {
            return _service.UnmarkAsFeaturedAsync(request.Id, cancellationToken);
        }
    }
}
