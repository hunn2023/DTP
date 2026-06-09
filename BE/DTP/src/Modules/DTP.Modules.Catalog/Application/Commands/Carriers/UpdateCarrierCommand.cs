using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.Carriers
{
    public class UpdateCarrierCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public string? Code { get; set; }

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public Guid CountryId { get; set; }

        public string? LogoUrl { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }

    public class UpdateCarrierCommandHandler
    : IRequestHandler<UpdateCarrierCommand, Result>
    {
        private readonly ICarrierService _carrierService;

        public UpdateCarrierCommandHandler(ICarrierService carrierService)
        {
            _carrierService = carrierService;
        }

        public async Task<Result> Handle(
            UpdateCarrierCommand request,
            CancellationToken cancellationToken)
        {
           return  await _carrierService.UpdateAsync(
                request.Id,
                request.Code,
                request.Name,
                request.Slug,
                request.CountryId,
                request.LogoUrl,
                request.SortOrder,
                request.IsActive,
                cancellationToken);

        }
    }
}
