using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.Carriers
{
    public class CreateCarrierCommand : IRequest<Result<Guid>>
    {
        public string? Code { get; set; }

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public Guid CountryId { get; set; }

        public string? LogoUrl { get; set; }

        public int SortOrder { get; set; }
    }

    public class CreateCarrierCommandHandler
    : IRequestHandler<CreateCarrierCommand, Result<Guid>>
    {
        private readonly ICarrierService _carrierService;

        public CreateCarrierCommandHandler(ICarrierService carrierService)
        {
            _carrierService = carrierService;
        }

        public async Task<Result<Guid>> Handle(
            CreateCarrierCommand request,
            CancellationToken cancellationToken)
        {
            return await _carrierService.CreateAsync(
                request.Code,
                request.Name,
                request.Slug,
                request.CountryId,
                request.LogoUrl,
                request.SortOrder,
                cancellationToken);
        }
    }
}
