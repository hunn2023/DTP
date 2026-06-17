using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.EsimPackages
{
    public class GetPublicEsimPackageBySlugQuery
        : IRequest<Result<List<EsimPackageDto>>>
    {
        public string Slug { get; set; }

        public GetPublicEsimPackageBySlugQuery(string slug)
        {
            Slug = slug;
        }
    }

    public class GetPublicEsimPackageBySlugQueryHandler
      : IRequestHandler<GetPublicEsimPackageBySlugQuery, Result<List<EsimPackageDto>>>
    {
        private readonly IEsimPackageService _esimPackageService;

        public GetPublicEsimPackageBySlugQueryHandler(
            IEsimPackageService esimPackageService)
        {
            _esimPackageService = esimPackageService;
        }

        public async Task<Result<List<EsimPackageDto>>> Handle(
            GetPublicEsimPackageBySlugQuery request,
            CancellationToken cancellationToken)
        {
            return await _esimPackageService.GetPublicBySlugAsync(
                request.Slug,
                cancellationToken);
        }
    }
}
