using DTP.Modules.Content.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Commands.Faqs
{
    public record EnableContentFaqCommand(Guid Id) : IRequest<bool>;


    public class EnableContentFaqCommandHandler
    : IRequestHandler<EnableContentFaqCommand, bool>
    {
        private readonly IContentFaqService _service;

        public EnableContentFaqCommandHandler(IContentFaqService service)
        {
            _service = service;
        }

        public Task<bool> Handle(
            EnableContentFaqCommand request,
            CancellationToken cancellationToken)
        {
            return _service.EnableAsync(request.Id, cancellationToken);
        }
    }
}
