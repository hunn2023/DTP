using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Faqs
{
    public record EnableContentFaqCommand(Guid Id) : IRequest<Result>;


    public class EnableContentFaqCommandHandler
    : IRequestHandler<EnableContentFaqCommand, Result>
    {
        private readonly IContentFaqService _service;

        public EnableContentFaqCommandHandler(IContentFaqService service)
        {
            _service = service;
        }

        public Task<Result> Handle(
            EnableContentFaqCommand request,
            CancellationToken cancellationToken)
        {
            return _service.EnableAsync(request.Id, cancellationToken);
        }
    }
}
