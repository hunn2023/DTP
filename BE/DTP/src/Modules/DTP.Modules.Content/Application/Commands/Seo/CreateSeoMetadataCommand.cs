using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using MediatR;

namespace DTP.Modules.Content.Application.Commands.Seo
{
    public record CreateSeoMetadataCommand(
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

    public class CreateSeoMetadataCommandHandler
    : IRequestHandler<CreateSeoMetadataCommand, SeoMetadataDto>
    {
        private readonly ISeoMetadataService _service;

        public CreateSeoMetadataCommandHandler(ISeoMetadataService service)
        {
            _service = service;
        }

        public Task<SeoMetadataDto> Handle(
            CreateSeoMetadataCommand request,
            CancellationToken cancellationToken)
        {
            return _service.CreateAsync(
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
