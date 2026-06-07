using DTP.Modules.Content.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Commands.Faqs
{
    public record DisableContentFaqCommand(Guid Id) : IRequest<bool>;


    public class DisableContentFaqCommandHandler
    : IRequestHandler<DisableContentFaqCommand, bool>
    {
        private readonly IContentFaqService _service;

        public DisableContentFaqCommandHandler(IContentFaqService service)
        {
            _service = service;
        }

        public Task<bool> Handle(
            DisableContentFaqCommand request,
            CancellationToken cancellationToken)
        {
            return _service.DisableAsync(request.Id, cancellationToken);
        }
    }
}
