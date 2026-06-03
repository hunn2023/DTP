using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Commands.Carriers
{
    public class CreateCarrierCommand : IRequest<Guid>
    {
        public string? Code { get; set; }

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public Guid CountryId { get; set; }

        public string? LogoUrl { get; set; }

        public int SortOrder { get; set; }
    }

    public class CreateCarrierCommandHandler
    : IRequestHandler<CreateCarrierCommand, Guid>
    {
        private readonly ICarrierService _carrierService;

        public CreateCarrierCommandHandler(ICarrierService carrierService)
        {
            _carrierService = carrierService;
        }

        public async Task<Guid> Handle(
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
