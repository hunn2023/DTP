using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Pages
{
    public record CreateContentPageCommand(
      string Code,
      string Title,
      string Slug,
      string? Summary,
      string Content,
      ContentPageStatus Status,
      int SortOrder) : IRequest<ContentPageDto>;

    public class CreateContentPageCommandHandler
    : IRequestHandler<CreateContentPageCommand, ContentPageDto>
    {
        private readonly IContentPageService _service;

        public CreateContentPageCommandHandler(IContentPageService service)
        {
            _service = service;
        }

        public Task<ContentPageDto> Handle(
            CreateContentPageCommand request,
            CancellationToken cancellationToken)
        {
            return _service.CreateAsync(
                request.Code,
                request.Title,
                request.Slug,
                request.Summary,
                request.Content,
                request.Status,
                request.SortOrder,
                cancellationToken);
        }
    }
}
