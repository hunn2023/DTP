using DTP.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Services
{
    public interface IProviderPackageSyncService
    {
        Task<Result<ProviderPackageSyncResult>> SyncAsync(
            string providerCode,
            CancellationToken cancellationToken = default);
    }

    public class ProviderPackageSyncResult
    {
        public int Total { get; set; }
        public int Synced { get; set; }
        public int Provisioned { get; set; }
        public int Failed { get; set; }

        public List<string> Errors { get; set; } = new();
    }
}
