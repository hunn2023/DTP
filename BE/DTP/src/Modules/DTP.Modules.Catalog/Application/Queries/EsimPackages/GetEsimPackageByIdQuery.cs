using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.EsimPackages
{
    public class GetEsimPackageByIdQuery : IRequest<EsimPackageDto?>
    {
        public Guid Id { get; set; }

        public GetEsimPackageByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetEsimPackageByIdQueryHandler
        : IRequestHandler<GetEsimPackageByIdQuery, EsimPackageDto?>
    {
        private readonly IEsimPackageService _esimPackageService;

        public GetEsimPackageByIdQueryHandler(
            IEsimPackageService esimPackageService)
        {
            _esimPackageService = esimPackageService;
        }

        public async Task<EsimPackageDto?> Handle(
            GetEsimPackageByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _esimPackageService.GetByIdAsync(
                request.Id,
                cancellationToken);
        }
    }
}
