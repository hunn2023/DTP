using DTP.Modules.Delivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Abstractions.Repositories
{
    public interface IEsimProfileRepository
    {
        Task<EsimProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<EsimProfile?> GetAvailableForOrderItemAsync(
            Guid productId,
            Guid? productVariantId,
            Guid? esimPackageId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsIccidAsync(
            string iccid,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            EsimProfile profile,
            CancellationToken cancellationToken = default);

        Task AddRangeAsync(
            List<EsimProfile> profiles,
            CancellationToken cancellationToken = default);

        void Update(EsimProfile profile);

        IQueryable<EsimProfile> Query();
    }
}
