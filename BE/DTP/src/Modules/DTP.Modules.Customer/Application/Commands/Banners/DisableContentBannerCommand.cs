using DTP.Modules.Content.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Banners
{

    public record DisableContentBannerCommand(Guid Id) : IRequest<bool>;


    public class DisableContentBannerCommandHandler
    : IRequestHandler<DisableContentBannerCommand, bool>
    {
        private readonly IContentBannerService _service;

        public DisableContentBannerCommandHandler(IContentBannerService service)
        {
            _service = service;
        }

        public Task<bool> Handle(
            DisableContentBannerCommand request,
            CancellationToken cancellationToken)
        {
            return _service.DisableAsync(request.Id, cancellationToken);
        }
    }
}
