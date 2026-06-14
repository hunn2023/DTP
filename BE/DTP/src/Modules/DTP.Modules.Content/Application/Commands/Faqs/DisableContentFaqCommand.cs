using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Faqs
{
    public record DisableContentFaqCommand(Guid Id) : IRequest<Result>;


    public class DisableContentFaqCommandHandler
    : IRequestHandler<DisableContentFaqCommand, Result>
    {
        private readonly IContentFaqService _service;

        public DisableContentFaqCommandHandler(IContentFaqService service)
        {
            _service = service;
        }

        public Task<Result> Handle(
            DisableContentFaqCommand request,
            CancellationToken cancellationToken)
        {
            return _service.DisableAsync(request.Id, cancellationToken);
        }
    }
}
