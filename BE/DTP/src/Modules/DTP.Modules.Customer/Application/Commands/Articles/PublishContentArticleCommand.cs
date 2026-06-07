using DTP.Modules.Content.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Articles
{
    public record PublishContentArticleCommand(Guid Id) : IRequest<bool>;


    public class PublishContentArticleCommandHandler
    : IRequestHandler<PublishContentArticleCommand, bool>
    {
        private readonly IContentArticleService _service;

        public PublishContentArticleCommandHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<bool> Handle(
            PublishContentArticleCommand request,
            CancellationToken cancellationToken)
        {
            return _service.PublishAsync(request.Id, cancellationToken);
        }
    }
}
