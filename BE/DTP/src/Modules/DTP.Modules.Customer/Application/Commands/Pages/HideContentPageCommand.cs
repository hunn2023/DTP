using DTP.Modules.Content.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Commands.Pages
{
    public record HideContentPageCommand(Guid Id) : IRequest<bool>;


    public class HideContentPageCommandHandler
    : IRequestHandler<HideContentPageCommand, bool>
    {
        private readonly IContentPageService _service;

        public HideContentPageCommandHandler(IContentPageService service)
        {
            _service = service;
        }

        public Task<bool> Handle(
            HideContentPageCommand request,
            CancellationToken cancellationToken)
        {
            return _service.HideAsync(request.Id, cancellationToken);
        }
    }
}
