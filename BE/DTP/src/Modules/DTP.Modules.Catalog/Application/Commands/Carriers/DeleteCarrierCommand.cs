using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Commands.Carriers
{
    public class DeleteCarrierCommand : IRequest<bool>
    {
        public Guid Id { get; }

        public DeleteCarrierCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteCarrierCommandHandler
    : IRequestHandler<DeleteCarrierCommand, bool>
    {
        private readonly ICarrierService _carrierService;

        public DeleteCarrierCommandHandler(ICarrierService carrierService)
        {
            _carrierService = carrierService;
        }

        public async Task<bool> Handle(
            DeleteCarrierCommand request,
            CancellationToken cancellationToken)
        {
            await _carrierService.DeleteAsync(
                request.Id,
                cancellationToken);

            return true;
        }
    }
}
