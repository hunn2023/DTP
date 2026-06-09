using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using MediatR;

namespace DTP.Modules.Content.Application.Queries.Pages
{
    public record GetContentPageByIdQuery(Guid Id) : IRequest<ContentPageDto?>;

    public class GetContentPageByIdQueryHandler
    : IRequestHandler<GetContentPageByIdQuery, ContentPageDto?>
    {
        private readonly IContentPageService _service;

        public GetContentPageByIdQueryHandler(IContentPageService service)
        {
            _service = service;
        }

        public Task<ContentPageDto?> Handle(
            GetContentPageByIdQuery request,
            CancellationToken cancellationToken)
        {
            return _service.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
