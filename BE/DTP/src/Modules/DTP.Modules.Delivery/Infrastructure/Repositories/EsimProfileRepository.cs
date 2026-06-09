using DTP.Modules.Delivery.Application.Abstractions.Repositories;
using DTP.Modules.Delivery.Domain.Entities;
using DTP.Modules.Delivery.Domain.Enums;
using DTP.Modules.Delivery.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Infrastructure.Repositories
{
    public class EsimProfileRepository : IEsimProfileRepository
    {
        private readonly DeliveryDbContext _context;

        public EsimProfileRepository(DeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<EsimProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.EsimProfiles
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<EsimProfile?> GetAvailableForOrderItemAsync(
            Guid productId,
            Guid? productVariantId,
            Guid? esimPackageId,
            CancellationToken cancellationToken = default)
        {
            return await _context.EsimProfiles
                .Where(x =>
                    !x.IsDeleted &&
                    x.Status == EsimProfileStatus.Available &&
                    x.ProductId == productId &&
                    x.ProductVariantId == productVariantId &&
                    x.EsimPackageId == esimPackageId)
                .OrderBy(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsIccidAsync(
            string iccid,
            CancellationToken cancellationToken = default)
        {
            return await _context.EsimProfiles
                .AnyAsync(x => x.Iccid == iccid && !x.IsDeleted, cancellationToken);
        }

        public async Task AddAsync(
            EsimProfile profile,
            CancellationToken cancellationToken = default)
        {
            await _context.EsimProfiles.AddAsync(profile, cancellationToken);
        }

        public async Task AddRangeAsync(
            List<EsimProfile> profiles,
            CancellationToken cancellationToken = default)
        {
            await _context.EsimProfiles.AddRangeAsync(profiles, cancellationToken);
        }

        public void Update(EsimProfile profile)
        {
            _context.EsimProfiles.Update(profile);
        }

        public IQueryable<EsimProfile> Query()
        {
            return _context.EsimProfiles.AsQueryable();
        }
    }
}
