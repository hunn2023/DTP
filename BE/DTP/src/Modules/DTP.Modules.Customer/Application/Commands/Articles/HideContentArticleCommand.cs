using DTP.Modules.Content.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Commands.Articles
{
    public record HideContentArticleCommand(Guid Id) : IRequest<bool>;


    public class HideContentArticleCommandHandler
    : IRequestHandler<HideContentArticleCommand, bool>
    {
        private readonly IContentArticleService _service;

        public HideContentArticleCommandHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<bool> Handle(
            HideContentArticleCommand request,
            CancellationToken cancellationToken)
        {
            return _service.HideAsync(request.Id, cancellationToken);
        }
    }
}
