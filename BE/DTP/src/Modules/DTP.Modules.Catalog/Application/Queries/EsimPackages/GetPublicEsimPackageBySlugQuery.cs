using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.EsimPackages
{
    public class GetPublicEsimPackageBySlugQuery
        : IRequest<Result<EsimPackageDto?>>
    {
        public string Slug { get; set; }

        public GetPublicEsimPackageBySlugQuery(string slug)
        {
            Slug = slug;
        }
    }

    public class GetPublicEsimPackageBySlugQueryHandler
      : IRequestHandler<GetPublicEsimPackageBySlugQuery, Result<EsimPackageDto?>>
    {
        private readonly IEsimPackageService _esimPackageService;

        public GetPublicEsimPackageBySlugQueryHandler(
            IEsimPackageService esimPackageService)
        {
            _esimPackageService = esimPackageService;
        }

        public async Task<Result<EsimPackageDto?>> Handle(
            GetPublicEsimPackageBySlugQuery request,
            CancellationToken cancellationToken)
        {
            return await _esimPackageService.GetPublicBySlugAsync(
                request.Slug,
                cancellationToken);
        }
    }
}
