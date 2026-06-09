using DTP.Modules.Content.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Banners
{
    public record EnableContentBannerCommand(Guid Id) : IRequest<bool>;



    public class EnableContentBannerCommandHandler
    : IRequestHandler<EnableContentBannerCommand, bool>
    {
        private readonly IContentBannerService _service;

        public EnableContentBannerCommandHandler(IContentBannerService service)
        {
            _service = service;
        }

        public Task<bool> Handle(
            EnableContentBannerCommand request,
            CancellationToken cancellationToken)
        {
            return _service.EnableAsync(request.Id, cancellationToken);
        }
    }
}
