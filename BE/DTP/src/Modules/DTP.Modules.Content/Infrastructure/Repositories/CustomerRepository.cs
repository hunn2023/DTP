
using DTP.Modules.Customer.Domain.Enums;
using DTP.Modules.Customer.Application.Abstractions.Repositories;
using DTP.Modules.Customer.Application.DTOs;
using DTP.Modules.Customer.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Customer.Infrastructure.Repositories
{
    public class CustomerRepository : RepositoryBase<Domain.Entities.Customer>, ICustomerRepository
    {
        private readonly CustomerDbContext _context;

        public CustomerRepository(CustomerDbContext context) : base(context)
        {
            _context = context;
        }

    

        public async Task<Domain.Entities.Customer?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted, cancellationToken);
        }

        public async Task<Domain.Entities.Customer?> GetDetailByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .Include(x => x.Addresses.Where(a => !a.IsDeleted))
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(
            string email,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            email = email.Trim().ToLower();

            return await _context.Customers
                .AnyAsync(x =>
                    !x.IsDeleted &&
                    x.Email == email &&
                    (!excludeId.HasValue || x.Id != excludeId.Value),
                    cancellationToken);
        }

        public async Task<bool> ExistsByUserIdAsync(
            Guid userId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .AnyAsync(x =>
                    !x.IsDeleted &&
                    x.UserId == userId &&
                    (!excludeId.HasValue || x.Id != excludeId.Value),
                    cancellationToken);
        }

        public async Task<PagedResultDto<CustomerListItemDto>> GetPagedAsync(
            string? keyword,
            CustomerStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Customers
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Email.Contains(keyword) ||
                    (x.FullName != null && x.FullName.Contains(keyword)) ||
                    (x.PhoneNumber != null && x.PhoneNumber.Contains(keyword)));
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CustomerListItemDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Email = x.Email,
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber,
                    Status = x.Status,
                    LastLoginAt = x.LastLoginAt,
                    CreatedAt = x.CreatedAt,
                    AddressCount = x.Addresses.Count(a => !a.IsDeleted)
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<CustomerListItemDto>(
                items,
                totalItems,
                pageIndex,
                pageSize);
        }
    }
}
