using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Articles
{
    public record HideContentArticleCommand(Guid Id) : IRequest<Result>;


    public class HideContentArticleCommandHandler
    : IRequestHandler<HideContentArticleCommand, Result>
    {
        private readonly IContentArticleService _service;

        public HideContentArticleCommandHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<Result> Handle(
            HideContentArticleCommand request,
            CancellationToken cancellationToken)
        {
            return _service.HideAsync(request.Id, cancellationToken);
        }
    }
}
