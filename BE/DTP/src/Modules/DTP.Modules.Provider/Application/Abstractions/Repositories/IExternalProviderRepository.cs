using DTP.Modules.Provider.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Repositories
{
    public interface IExternalProviderRepository
    {
        Task<ExternalProvider?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<ExternalProvider?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        Task<List<ExternalProvider>> GetActiveAsync(CancellationToken cancellationToken = default);

        Task<bool> ExistsCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);

        Task AddAsync(ExternalProvider provider, CancellationToken cancellationToken = default);

        void Update(ExternalProvider provider);

        void Remove(ExternalProvider provider);
    }
}
