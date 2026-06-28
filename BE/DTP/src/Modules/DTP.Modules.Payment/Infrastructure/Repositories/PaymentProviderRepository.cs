using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Domain.Entities;
using DTP.Modules.Payment.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Repositories
{
    public class PaymentProviderRepository : RepositoryBase<PaymentProvider>, IPaymentProviderRepository
    {
        private readonly PaymentDbContext _dbContext;

        public PaymentProviderRepository(PaymentDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<PaymentProvider>> GetPublicActiveAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.PaymentProviders
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<PaymentProvider>> GetAdminListAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.PaymentProviders
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }



        public async Task<PaymentProvider?> GetActiveByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return await _dbContext.PaymentProviders
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Code == normalizedCode && x.IsActive && !x.IsDeleted,
                    cancellationToken);
        }

        public async Task<PaymentProvider?> GetDefaultActiveAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.PaymentProviders
                .AsNoTracking()
                .Where(x => x.IsActive && x.IsDefault)
                .OrderBy(x => x.SortOrder)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<PaymentProvider>> GetAllTrackingAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.PaymentProviders
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ToListAsync(cancellationToken);
        }

        public async Task<PaymentProvider?> GetAlternativeActiveProviderAsync(
            Guid excludeId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.PaymentProviders
                .AsNoTracking()
                .Where(x => x.Id != excludeId && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> HasActiveDefaultAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.PaymentProviders
                .AsNoTracking()
                .AnyAsync(x => x.IsActive && x.IsDefault, cancellationToken);
        }


        public Task<PaymentProvider?> GetDefaultAvailableAsync(
           string currency,
           decimal amount,
           CancellationToken cancellationToken = default)
        {
            var normalizedCurrency = string.IsNullOrWhiteSpace(currency)
                ? "VND"
                : currency.Trim().ToUpperInvariant();

            return _dbContext.PaymentProviders
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Where(x => x.Currency == normalizedCurrency)
                .Where(x => !x.MinAmount.HasValue || amount >= x.MinAmount.Value)
                .Where(x => !x.MaxAmount.HasValue || amount <= x.MaxAmount.Value)
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<PaymentProvider?> GetByCodeAsync(
              string code,
              CancellationToken cancellationToken = default)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return _dbContext.PaymentProviders
                .FirstOrDefaultAsync(x => x.Code == normalizedCode, cancellationToken);
        }
    }
}
