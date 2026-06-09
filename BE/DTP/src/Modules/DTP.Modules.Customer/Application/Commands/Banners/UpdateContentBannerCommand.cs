using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Banners
{
    public record UpdateContentBannerCommand(
       Guid Id,
       string Title,
       string ImageUrl,
       string? MobileImageUrl,
       string? LinkUrl,
       string? Description,
       ContentBannerPosition Position,
       DateTime? StartDate,
       DateTime? EndDate,
       int SortOrder,
       bool IsActive) : IRequest<ContentBannerDto>;


    public class UpdateContentBannerCommandHandler
    : IRequestHandler<UpdateContentBannerCommand, ContentBannerDto>
    {
        private readonly IContentBannerService _service;

        public UpdateContentBannerCommandHandler(IContentBannerService service)
        {
            _service = service;
        }

        public Task<ContentBannerDto> Handle(
            UpdateContentBannerCommand request,
            CancellationToken cancellationToken)
        {
            return _service.UpdateAsync(
                request.Id,
                request.Title,
                request.ImageUrl,
                request.MobileImageUrl,
                request.LinkUrl,
                request.Description,
                request.Position,
                request.StartDate,
                request.EndDate,
                request.SortOrder,
                request.IsActive,
                cancellationToken);
        }
    }
}
