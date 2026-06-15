using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface IEsimPackageCoverageRepository : IRepositoryBase<EsimPackageCoverage>
    {
        Task<IReadOnlyList<EsimPackageCoverage>> GetByEsimPackageIdAsync(
            Guid esimPackageId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            Guid esimPackageId,
            Guid countryId,
            CancellationToken cancellationToken = default);


        Task DeleteByEsimPackageIdAsync(
            Guid esimPackageId,
            CancellationToken cancellationToken = default);
    }
}
