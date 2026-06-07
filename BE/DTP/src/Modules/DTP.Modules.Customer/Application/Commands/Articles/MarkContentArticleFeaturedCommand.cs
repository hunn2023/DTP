using DTP.Modules.Content.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Commands.Articles
{
    public record MarkContentArticleFeaturedCommand(Guid Id) : IRequest<bool>;

    public class MarkContentArticleFeaturedCommandHandler
    : IRequestHandler<MarkContentArticleFeaturedCommand, bool>
    {
        private readonly IContentArticleService _service;

        public MarkContentArticleFeaturedCommandHandler(IContentArticleService service)
        {
            _service = service;
        }

        public Task<bool> Handle(
            MarkContentArticleFeaturedCommand request,
            CancellationToken cancellationToken)
        {
            return _service.MarkAsFeaturedAsync(request.Id, cancellationToken);
        }
    }
}
