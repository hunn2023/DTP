using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.EsimPackages
{
    public record GetEsimDestinationDetailQuery(string CountrySlug)
        : IRequest<Result<EsimDestinationDetailDto?>>;

    public class GetEsimDestinationDetailQueryHandler
        : IRequestHandler<GetEsimDestinationDetailQuery, Result<EsimDestinationDetailDto?>>
    {
        private readonly IEsimPackageService _esimPackageService;

        public GetEsimDestinationDetailQueryHandler(IEsimPackageService esimPackageService)
        {
            _esimPackageService = esimPackageService;
        }

        public Task<Result<EsimDestinationDetailDto?>> Handle(
            GetEsimDestinationDetailQuery request,
            CancellationToken cancellationToken)
        {
            return _esimPackageService.GetDestinationDetailAsync(
                request.CountrySlug,
                cancellationToken);
        }
    }
}
