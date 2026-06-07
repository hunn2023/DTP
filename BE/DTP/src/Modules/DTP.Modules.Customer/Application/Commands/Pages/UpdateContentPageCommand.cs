using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Pages
{
    public record UpdateContentPageCommand(
       Guid Id,
       string Title,
       string Slug,
       string? Summary,
       string Content,
       string? ThumbnailUrl,
       ContentPageStatus Status,
       int SortOrder) : IRequest<ContentPageDto>;

    public class UpdateContentPageCommandHandler
    : IRequestHandler<UpdateContentPageCommand, ContentPageDto>
    {
        private readonly IContentPageService _service;

        public UpdateContentPageCommandHandler(IContentPageService service)
        {
            _service = service;
        }

        public Task<ContentPageDto> Handle(
            UpdateContentPageCommand request,
            CancellationToken cancellationToken)
        {
            return _service.UpdateAsync(
                request.Id,
                request.Title,
                request.Slug,
                request.Summary,
                request.Content,
                request.ThumbnailUrl,
                request.Status,
                request.SortOrder,
                cancellationToken);
        }
    }
}
