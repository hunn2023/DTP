using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Articles
{
    public record PublishContentArticleCommand(Guid Id) : IRequest<Result>;


    public class PublishContentArticleCommandHandler
    : IRequestHandler<PublishContentArticleCommand, Result>
    {
        private readonly IContentArticleService _service;

        public PublishContentArticleCommandHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<Result> Handle(
            PublishContentArticleCommand request,
            CancellationToken cancellationToken)
        {
            return _service.PublishAsync(request.Id, cancellationToken);
        }
    }
}
