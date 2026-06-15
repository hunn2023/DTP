using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Commands.Providers
{
    public class SyncProviderPackagesCommand : IRequest<Result<ProviderPackageSyncResult>>
    {
        public string ProviderCode { get; set; } = default!;
    }

    public class SyncProviderPackagesCommandHandler
        : IRequestHandler<SyncProviderPackagesCommand, Result<ProviderPackageSyncResult>>
    {
        private readonly IProviderPackageSyncService _syncService;

        public SyncProviderPackagesCommandHandler(
            IProviderPackageSyncService syncService)
        {
            _syncService = syncService;
        }

        public async Task<Result<ProviderPackageSyncResult>> Handle(
            SyncProviderPackagesCommand request,
            CancellationToken cancellationToken)
        {
            return await _syncService.SyncAsync(
                request.ProviderCode,
                cancellationToken);
        }
    }
}
