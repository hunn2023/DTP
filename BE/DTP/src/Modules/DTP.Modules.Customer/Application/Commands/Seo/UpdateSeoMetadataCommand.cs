using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Seo
{
    public record UpdateSeoMetadataCommand(
      Guid Id,
      string EntityType,
      Guid? EntityId,
      string? RoutePath,
      string MetaTitle,
      string? MetaDescription,
      string? MetaKeywords,
      string? CanonicalUrl,
      string? OgTitle,
      string? OgDescription,
      string? OgImageUrl,
      string? Robots) : IRequest<SeoMetadataDto>;


    public class UpdateSeoMetadataCommandHandler
    : IRequestHandler<UpdateSeoMetadataCommand, SeoMetadataDto>
    {
        private readonly ISeoMetadataService _service;

        public UpdateSeoMetadataCommandHandler(ISeoMetadataService service)
        {
            _service = service;
        }

        public Task<SeoMetadataDto> Handle(
            UpdateSeoMetadataCommand request,
            CancellationToken cancellationToken)
        {
            return _service.UpdateAsync(
                request.Id,
                request.EntityType,
                request.EntityId,
                request.RoutePath,
                request.MetaTitle,
                request.MetaDescription,
                request.MetaKeywords,
                request.CanonicalUrl,
                request.OgTitle,
                request.OgDescription,
                request.OgImageUrl,
                request.Robots,
                cancellationToken);
        }
    }
}
