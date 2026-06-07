using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Repositories
{
    public class PaymentUnitOfWork : IPaymentUnitOfWork
    {
        private readonly PaymentDbContext _context;

        public PaymentUnitOfWork(PaymentDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
