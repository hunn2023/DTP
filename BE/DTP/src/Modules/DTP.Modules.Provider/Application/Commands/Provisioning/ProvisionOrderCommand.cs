using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Commands.Provisioning
{
    public class ProvisionOrderCommand : IRequest<ProviderProvisionResultDto>
    {
        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public List<ProviderProvisionOrderItemInput> Items { get; set; } = new();
    }

    public class ProvisionOrderCommandHandler
        : IRequestHandler<ProvisionOrderCommand, ProviderProvisionResultDto>
    {
        private readonly IProviderProvisioningService _service;

        public ProvisionOrderCommandHandler(IProviderProvisioningService service)
        {
            _service = service;
        }

        public async Task<ProviderProvisionResultDto> Handle(
            ProvisionOrderCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.ProvisionOrderAsync(
                request.OrderId,
                request.OrderCode,
                request.Items,
                cancellationToken);
        }
    }
}
