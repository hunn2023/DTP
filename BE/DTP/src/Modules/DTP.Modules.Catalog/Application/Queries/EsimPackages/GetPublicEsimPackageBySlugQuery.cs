using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.EsimPackages
{
    public class GetPublicEsimPackageBySlugQuery
        : IRequest<EsimPackageDto?>
    {
        public string Slug { get; set; }

        public GetPublicEsimPackageBySlugQuery(string slug)
        {
            Slug = slug;
        }
    }

    public class GetPublicEsimPackageBySlugQueryHandler
      : IRequestHandler<GetPublicEsimPackageBySlugQuery, EsimPackageDto?>
    {
        private readonly IEsimPackageService _esimPackageService;

        public GetPublicEsimPackageBySlugQueryHandler(
            IEsimPackageService esimPackageService)
        {
            _esimPackageService = esimPackageService;
        }

        public async Task<EsimPackageDto?> Handle(
            GetPublicEsimPackageBySlugQuery request,
            CancellationToken cancellationToken)
        {
            return await _esimPackageService.GetPublicBySlugAsync(
                request.Slug,
                cancellationToken);
        }
    }
}
