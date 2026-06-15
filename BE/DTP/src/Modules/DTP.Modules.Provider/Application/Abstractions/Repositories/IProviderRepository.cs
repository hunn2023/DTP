using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Abstractions.Repositories
{
    public interface IProviderRepository
    {
        Task<Domain.Entities.Provider?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Domain.Entities.Provider?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Domain.Entities.Provider provider,
            CancellationToken cancellationToken = default);
    }
}
