using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Commands.Provisioning
{
    public class RetryProviderOrderCommand : IRequest<ProviderProvisionResultDto>
    {
        public Guid ProviderOrderId { get; set; }
    }

    public class RetryProviderOrderCommandHandler
        : IRequestHandler<RetryProviderOrderCommand, ProviderProvisionResultDto>
    {
        private readonly IProviderProvisioningService _service;

        public RetryProviderOrderCommandHandler(IProviderProvisioningService service)
        {
            _service = service;
        }

        public async Task<ProviderProvisionResultDto> Handle(
            RetryProviderOrderCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.RetryAsync(
                request.ProviderOrderId,
                cancellationToken);
        }
    }
}
