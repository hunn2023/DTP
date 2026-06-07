using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.EsimPackages
{
    public class UpdateEsimPackageCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public Guid CountryId { get; set; }

        public Guid CarrierId { get; set; }

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public int DataAmount { get; set; }

        public string DataUnit { get; set; } = default!;

        public int ValidityDays { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; } = "USD";

        public bool IsUnlimited { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }

    public class UpdateEsimPackageCommandHandler
        : IRequestHandler<UpdateEsimPackageCommand, Result>
    {
        private readonly IEsimPackageService _esimPackageService;

        public UpdateEsimPackageCommandHandler(
            IEsimPackageService esimPackageService)
        {
            _esimPackageService = esimPackageService;
        }

        public async Task<Result> Handle(
            UpdateEsimPackageCommand request,
            CancellationToken cancellationToken)
        {
            return await _esimPackageService.UpdateAsync(
                request,
                cancellationToken);
        }
    }
}
