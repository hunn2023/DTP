using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Commands.Banners
{
    public record CreateContentBannerCommand(
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


    public class CreateContentBannerCommandHandler
    : IRequestHandler<CreateContentBannerCommand, ContentBannerDto>
    {
        private readonly IContentBannerService _service;

        public CreateContentBannerCommandHandler(IContentBannerService service)
        {
            _service = service;
        }

        public Task<ContentBannerDto> Handle(
            CreateContentBannerCommand request,
            CancellationToken cancellationToken)
        {
            return _service.CreateAsync(
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
