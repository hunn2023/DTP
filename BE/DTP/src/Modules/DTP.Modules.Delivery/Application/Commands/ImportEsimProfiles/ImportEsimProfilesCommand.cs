using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Commands.ImportEsimProfiles
{
    public class ImportEsimProfilesCommand : IRequest<int>
    {
        public List<ImportEsimProfileDto> Items { get; set; } = new();
    }

    public class ImportEsimProfilesCommandHandler
       : IRequestHandler<ImportEsimProfilesCommand, int>
    {
        private readonly IDeliveryService _deliveryService;

        public ImportEsimProfilesCommandHandler(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public async Task<int> Handle(
            ImportEsimProfilesCommand request,
            CancellationToken cancellationToken)
        {
            return await _deliveryService.ImportEsimProfilesAsync(
                request.Items,
                cancellationToken);
        }
    }
}
