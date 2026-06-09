
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.EsimPackages
{
    public class CreateEsimPackageCommand : IRequest<Result<Guid>>
    {
        public Guid ProductId { get; set; }

        public Guid ProductVariantId { get; set; }

        public Guid ProviderId { get; set; }

        public Guid CountryId { get; set; }

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public string ProviderPackageCode { get; set; } = default!;

        public decimal? DataAmount { get; set; }

        public string DataUnit { get; set; } = default!;

        public int ValidityDays { get; set; }

        public bool IsUnlimited { get; set; }

        public string CoverageType { get; set; } = default!;

        public string? CoverageDescription { get; set; }

        public string ActivationPolicy { get; set; } = default!;

        public string? SpeedPolicy { get; set; }

        public bool HotspotSupported { get; set; }

        public bool PhoneNumberSupported { get; set; }

        public bool SmsSupported { get; set; }

        public bool KycRequired { get; set; }

        public string QrDeliveryType { get; set; } = default!;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public List<Guid> CarrierIds { get; set; } = new();
    }
    public class CreateEsimPackageCommandHandler
       : IRequestHandler<CreateEsimPackageCommand, Result<Guid>>
    {
        private readonly IEsimPackageService _esimPackageService;

        public CreateEsimPackageCommandHandler(
            IEsimPackageService esimPackageService)
        {
            _esimPackageService = esimPackageService;
        }

        public async Task<Result<Guid>> Handle(
            CreateEsimPackageCommand request,
            CancellationToken cancellationToken)
        {
            return await _esimPackageService.CreateAsync(
                request,
                cancellationToken);
        }
    }
}
