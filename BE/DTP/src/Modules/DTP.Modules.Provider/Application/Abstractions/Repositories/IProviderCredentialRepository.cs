using DTP.Modules.Provider.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Repositories
{
    public interface IProviderCredentialRepository
    {
        Task<ProviderCredential?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<ProviderCredential>> GetByProviderIdAsync(
            Guid providerId,
            CancellationToken cancellationToken = default);

        Task<ProviderCredential?> GetActiveByKeyAsync(
            Guid providerId,
            string key,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsKeyAsync(
            Guid providerId,
            string key,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            ProviderCredential credential,
            CancellationToken cancellationToken = default);

        void Update(ProviderCredential credential);

        void Remove(ProviderCredential credential);
    }
}
