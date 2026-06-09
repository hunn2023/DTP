using DTP.Modules.Auth.Application.Abstractions.Repositories;
using DTP.Modules.Auth.Domain.Entities;
using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Infrastructure.Repositories
{
    public class PendingRegistrationRepository
     : RepositoryBase<PendingRegistration>,
       IPendingRegistrationRepository
    {
        private readonly AuthDbContext _context;

        public PendingRegistrationRepository(AuthDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<PendingRegistration?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            email = email.Trim().ToLower();

            return await _context.PendingRegistrations
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public async Task DeleteByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            email = email.Trim().ToLower();

            var items = await _context.PendingRegistrations
                .Where(x => x.Email == email)
                .ToListAsync(cancellationToken);

            _context.PendingRegistrations.RemoveRange(items);
        }
    }
}
